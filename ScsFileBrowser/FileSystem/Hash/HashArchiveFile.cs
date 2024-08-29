using System.Runtime.InteropServices;
using System.Text;
using ScsFileBrowser.Logging;
using ScsFileBrowser.Utils;

namespace ScsFileBrowser.FileSystem.Hash;

internal class HashArchiveFile : ArchiveFile
{
    /// <summary>
    ///     Hashing method used in scs files, 'CITY' as utf-8 bytes
    /// </summary>
    private const uint HashMethod = 1498696003;

    private const ushort EntryV1BlockSize = 0x20;
    private const ushort EntryV2BlockSize = 0x10;
    private const ushort MetadataV2BlockSize = 0x04;

    private readonly ushort[] _supportedHashVersion = {1, 2};

    private BaseHashArchiveHeader _hashHeader;

    /// <summary>
    ///     Represents a hash archive file.
    ///     Need to run <see cref="Parse" /> after to actually read the file contents
    /// </summary>
    /// <param name="path">Path to the hash file</param>
    internal HashArchiveFile(string path) : base(path)
    {
    }

    /// <summary>
    ///     Does minimal validation on the file and reads all the <see cref="Entry">Entries</see>
    /// </summary>
    /// <returns>Whether parsing was successful or not</returns>
    internal override bool Parse()
    {
        if (!File.Exists(Path))
        {
            Logger.Instance.Error($"Could not find file {Path}");
            return false;
        }

        Br = new BinaryReader(File.OpenRead(Path));
        _hashHeader = new BaseHashArchiveHeader
        {
            Magic = MemoryUtils.ReadUInt32(Br, 0x0),
            Version = MemoryUtils.ReadUInt16(Br, 0x04),
            Salt = MemoryUtils.ReadUInt16(Br, 0x06),
            HashMethod = MemoryUtils.ReadUInt32(Br, 0x08),
            EntryCount = MemoryUtils.ReadUInt32(Br, 0x0C)
        };

        if (_hashHeader.Magic != Consts.ScsMagic)
        {
            Logger.Instance.Error("Incorrect FileModel Structure");
            return false;
        }

        if (_hashHeader.HashMethod != HashMethod)
        {
            Logger.Instance.Error("Incorrect Hash Method");
            return false;
        }

        if (!_supportedHashVersion.Contains(_hashHeader.Version))
        {
            Logger.Instance.Error("Unsupported Hash Version");
            return false;
        }

        if (_hashHeader.Version == 1)
        {
            var startOffset = MemoryUtils.ReadUInt32(Br, 0x10);

            Br.BaseStream.Seek(startOffset, SeekOrigin.Begin);
            var entriesRaw =
                Br.ReadBytes((int) _hashHeader.EntryCount *
                             EntryV1BlockSize); // read all entries at once for performance

            for (var i = 0; i < _hashHeader.EntryCount; i++)
            {
                var offset = (uint) i * EntryV1BlockSize;
                var entry = new HashEntryV1(this)
                {
                    Hash = MemoryUtils.ReadUInt64(entriesRaw, offset),
                    Offset = MemoryUtils.ReadUInt64(entriesRaw, offset + 0x08),
                    Flags = MemoryUtils.ReadUInt32(entriesRaw, offset + 0x10),
                    Crc = MemoryUtils.ReadUInt32(entriesRaw, offset + 0x14),
                    Size = MemoryUtils.ReadUInt32(entriesRaw, offset + 0x18),
                    CompressedSize = MemoryUtils.ReadUInt32(entriesRaw, offset + 0x1C)
                };

                if (entry.IsDirectory())
                {
                    var dir = GetDirectory(entry.GetHash());
                    if (dir == null)
                    {
                        dir = new UberDirectory
                        {
                            Entry = entry
                        };
                        Directories[entry.GetHash()] = dir;
                    }

                    var lines = Encoding.UTF8.GetString(entry.Read())
                        .Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (line == "") continue;

                        if (line.StartsWith("*")) // dir
                            dir.AddSubDirName(line[1..]);
                        else
                            dir.AddSubFileName(line);
                    }
                }
                else
                    Files.Add(entry.GetHash(), new UberFile(entry));

                Entries.Add(entry.Hash, entry);
            }
        }
        else if (_hashHeader.Version == 2)
        {
            var entryTableBlockSize = MemoryUtils.ReadUInt32(Br, 0x10);
            var metadataCount = MemoryUtils.ReadUInt32(Br, 0x14);
            var metadataTableBlockSize = MemoryUtils.ReadUInt32(Br, 0x18);
            var entryTableBlockOffset = MemoryUtils.ReadInt64(Br, 0x1C);
            var metadataTableBlockOffset = MemoryUtils.ReadInt64(Br, 0x24);

            Br.BaseStream.Seek(entryTableBlockOffset, SeekOrigin.Begin);
            var entriesBlockRaw =
                Br.ReadBytes((int) entryTableBlockSize);

            var rawEntries = MemoryUtils.DecompressZlib(entriesBlockRaw, _hashHeader.EntryCount * EntryV2BlockSize);

            Br.BaseStream.Seek(metadataTableBlockOffset, SeekOrigin.Begin);
            var metadataBlockRaw =
                Br.ReadBytes((int) metadataTableBlockSize);

            var rawMetadataBytes = MemoryUtils.DecompressZlib(metadataBlockRaw, metadataCount * MetadataV2BlockSize);
            var metadataTable = MemoryMarshal.Cast<byte, uint>(rawMetadataBytes.AsSpan());
            for (var i = 0; i < _hashHeader.EntryCount; i++)
            {
                var offset = (uint) (i * EntryV2BlockSize);
                var metadataStartIndex = MemoryUtils.ReadInt32(rawEntries, offset + 0x08);
                var entry = new HashEntryV2(this)
                {
                    Hash = MemoryUtils.ReadUInt64(rawEntries, offset),
                    Offset = metadataTable[metadataStartIndex + 4] * 0x10ul,
                    Flags = MemoryUtils.ReadUInt32(rawEntries, offset + 0x0c),
                    // Crc = MemoryUtils.ReadUInt32(rawEntries, offset + 0x14),
                    Size = metadataTable[metadataStartIndex + 2] & 0xffff,
                    CompressedSize = metadataTable[metadataStartIndex + 1] & 0xffff
                };

                if (entry.IsDirectory())
                {
                    var dir = GetDirectory(entry.GetHash());
                    if (dir == null)
                    {
                        dir = new UberDirectory
                        {
                            Entry = entry
                        };
                        Directories[entry.GetHash()] = dir;
                    }

                    var dirSubData = entry.Read();
                    var subItemCount = MemoryUtils.ReadUInt32(dirSubData, 0);
                    var strOffset = (int) (4 + subItemCount);
                    for (var j = 0; j < subItemCount; j++)
                    {
                        var length = dirSubData[4 + j];
                        try
                        {
                            var subItem = MemoryUtils.ReadString(dirSubData, strOffset, length);
                            if (subItem.StartsWith('/'))
                                dir.AddSubDirName(subItem[1..]);
                            else
                                dir.AddSubFileName(subItem);
                            strOffset += length;
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.Debug($"Hash: {entry.Hash}, {Path}");
                            break;
                        }
                    }
                }
                else
                    Files.Add(entry.GetHash(), new UberFile(entry));

                Entries.Add(entry.Hash, entry);
            }
        }
        else
        {
            Logger.Instance.Error($"Incompatible HashFs format {_hashHeader.Version} was found.");
            return false;
        }

        Logger.Instance.Info(
            $"Mounted '{System.IO.Path.GetFileName(Path)}' with {_hashHeader.EntryCount} entries");
        return true;
    }

    /// <summary>
    ///     Tries to find the directory by the given path
    /// </summary>
    /// <param name="path">Path for the wanted directory</param>
    /// <returns>
    ///     <see cref="UberDirectory" /> for the given path
    ///     <para>Null if path was not found</para>
    /// </returns>
    internal override UberDirectory? GetDirectory(string path)
    {
        return GetDirectory(CityHash.CityHash64((_hashHeader.Salt != 0
            ? _hashHeader.Salt.ToString()
            : "") + PathUtils.EnsureLocalPath(path)));
    }

    /// <summary>
    ///     Tries to find the file by a given path
    /// </summary>
    /// <param name="path">Path for the wanted file</param>
    /// <returns>
    ///     <see cref="UberFile" /> for the given path
    ///     <para>Null if path was not found</para>
    /// </returns>
    internal override UberFile? GetFile(string path)
    {
        return GetFile(CityHash.CityHash64((_hashHeader.Salt != 0
            ? _hashHeader.Salt.ToString()
            : "") + PathUtils.EnsureLocalPath(path)));
    }
}
