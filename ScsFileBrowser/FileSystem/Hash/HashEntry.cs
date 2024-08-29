using ComponentAce.Compression.Libs.zlib;
using Ionic.Zlib;
using ScsFileBrowser.Logging;
using ScsFileBrowser.Utils;

namespace ScsFileBrowser.FileSystem.Hash;

internal class HashEntryV1 : Entry
{
    internal HashEntryV1(HashArchiveFile fsFile) : base(fsFile)
    {
    }

    /// <summary>
    ///     <para>0001 => Directory</para>
    ///     <para>0010 => Compressed</para>
    /// </summary>
    internal uint Flags { get; set; }

    internal uint Crc { get; set; }

    internal override byte[] Read()
    {
        var buff = MemoryUtils.ReadBytes(GetArchiveFile().Br, (long) Offset, (int) CompressedSize);
        return IsCompressed() ? Inflate(buff) : buff;
    }

    protected override byte[] Inflate(byte[] data)
    {
        try
        {
            var uncompressed = new byte[Size];

            var zs = new ZStream();
            zs.inflateInit(15);
            zs.next_in = data;
            zs.avail_in = data.Length;
            zs.next_out = uncompressed;
            zs.avail_out = uncompressed.Length;


            zs.inflate(zlibConst.Z_DEFAULT_COMPRESSION);
            zs.inflateEnd();

            return uncompressed;
        }
        catch (Exception e)
        {
            Logger.Instance.Error(
                $"Could not inflate hash entry: 0x{Hash:X}, of '{GetArchiveFile().GetPath()}', reason: {e.Message}");
            return Array.Empty<byte>();
        }
    }

    internal override bool IsDirectory()
    {
        return MemoryUtils.IsBitSet(Flags, 0);
    }

    internal override bool IsCompressed()
    {
        return MemoryUtils.IsBitSet(Flags, 1);
    }
}

internal class HashEntryV2 : Entry
{
    internal HashEntryV2(HashArchiveFile fsFile) : base(fsFile)
    {
    }

    internal uint Flags { get; set; }

    internal override byte[] Read()
    {
        var buff = MemoryUtils.ReadBytes(GetArchiveFile().Br, (long) Offset, (int) CompressedSize);
        return IsCompressed() ? Inflate(buff) : buff;
    }

    protected override byte[] Inflate(byte[] data)
    {
        try
        {
            return DeflateStream.UncompressBuffer(data[2..]);
        }
        catch (Exception e)
        {
            Logger.Instance.Error(
                $"Could not inflate hash entry: 0x{Hash:X}, of '{GetArchiveFile().GetPath()}', reason: {e.Message}");
            return Array.Empty<byte>();
        }
    }

    internal override bool IsDirectory()
    {
        return MemoryUtils.IsBitSet(Flags >> 16, 0);
    }

    internal override bool IsCompressed()
    {
        return Size != CompressedSize;
    }
}
