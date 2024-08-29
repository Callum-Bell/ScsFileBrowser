using ScsFileBrowser.Logging;
using ScsFileBrowser.Utils;

namespace ScsFileBrowser.FileSystem.Zip;

internal class ZipArchiveFile : ArchiveFile
{
    private readonly byte[] _eocdSignature = {0x50, 0x4b, 0x05, 0x06};

    internal ZipArchiveFile(string path) : base(path)
    {
    }

    private int GetEndOfCentralDirectory()
    {
        byte[] data;
        if (Br.BaseStream.Length < 1001)
            data = Br.ReadBytes((int) Br.BaseStream.Length);
        else
        {
            Br.BaseStream.Position = Br.BaseStream.Length - 1000;
            data = Br.ReadBytes(1000);
        }

        for (var i = data.Length - 5; i >= 0; --i)
        {
            if (data.Skip(i).Take(_eocdSignature.Length).SequenceEqual(_eocdSignature))
                return data.Length - i;
        }

        return 0;
    }

    internal override bool Parse()
    {
        if (!File.Exists(Path))
        {
            Logger.Instance.Error($"Could not find file {Path}");
            return false;
        }

        Br = new BinaryReader(File.OpenRead(Path));

        var eocdOffset = Br.BaseStream.Length - GetEndOfCentralDirectory();

        if (eocdOffset == Br.BaseStream.Length)
        {
            Logger.Instance.Error($"Could not find End of Central Directory in zip file, '{Path}'");
            return false;
        }

        var entryCount = MemoryUtils.ReadUInt16(Br, eocdOffset + 10);
        var cdOffset = MemoryUtils.ReadUInt32(Br, eocdOffset + 16);

        var fileOffset = cdOffset;

        for (var i = 0; i < entryCount; i++)
        {
            var entry = new ZipEntry(this)
            {
                CompressedSize = MemoryUtils.ReadUInt32(Br, fileOffset += 0x14),
                Size = MemoryUtils.ReadUInt32(Br, fileOffset += 0x04) // 0x04(CompressedSize)
            };

            var nameLen = MemoryUtils.ReadUInt16(Br, fileOffset += 0x04); // 0x04(Size)

            var extraFieldLength = MemoryUtils.ReadUInt16(Br, fileOffset += 0x02); // 0x02(nameLen)
            var fileCommentLength = MemoryUtils.ReadUInt16(Br, fileOffset += 0x02); // 0x02(extraFieldLength)
            var offsetLocalHeader =
                MemoryUtils.ReadUInt32(Br,
                    fileOffset +=
                        0x02 + 0x08); // 0x02(fileCommentLength) + 0x08(deDiskNumberStart + deInternalAttributes + deExternalAttributes)

            var name = MemoryUtils.ReadString(Br, fileOffset += 0x04, nameLen);
            if (name.EndsWith("/")) name = name[..^1];
            entry.Hash = CityHash.CityHash64(name);

            fileOffset += (uint) nameLen + extraFieldLength + fileCommentLength;

            if (entry.Size != 0)
            {
                var prevOffset = fileOffset;

                fileOffset = offsetLocalHeader + 0x1A; // offset to name length

                var localNameLength = MemoryUtils.ReadUInt16(Br, fileOffset);

                if (nameLen != localNameLength)
                {
                    Logger.Instance.Debug(
                        $"Local name length is different than CD one for zip entry {i} '{name}' in '{Path}'");
                }

                var localExtraField = MemoryUtils.ReadUInt16(Br, fileOffset += 0x02); // 0x02(localNameLength)

                entry.Offset = fileOffset + 0x02u + nameLen + localExtraField; // Offset to data

                fileOffset = prevOffset;
            }

            var parentDirPath = System.IO.Path.GetDirectoryName(name)?.Replace('\\', '/');
            if (parentDirPath == null) continue;
            var parentDirHash = CityHash.CityHash64(parentDirPath);

            UberDirectory parentDir;

            if (Directories.ContainsKey(parentDirHash))
                parentDir = Directories[parentDirHash];
            else
            {
                parentDir = new UberDirectory();
                Directories.Add(parentDirHash, parentDir);
            }

            if (entry.IsDirectory())
            {
                var dir = new UberDirectory
                {
                    Entry = entry
                };
                Directories.Add(entry.GetHash(), dir);

                parentDir.AddSubDirName(System.IO.Path.GetFileName(name));
            }
            else
            {
                if (Files.ContainsKey(entry.Hash))
                    Files[entry.Hash] = new UberFile(entry);
                else
                {
                    parentDir.AddSubFileName(System.IO.Path.GetFileName(name));
                    Files.Add(entry.Hash, new UberFile(entry));
                }
            }

            Entries.Add(entry.Hash, entry);
        }

        Logger.Instance.Info($"Mounted '{System.IO.Path.GetFileName(Path)}' with {entryCount} entries");
        return true;
    }
}
