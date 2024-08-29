using System.IO.Compression;
using ScsFileBrowser.Utils;

namespace ScsFileBrowser.FileSystem.Zip;

internal class ZipEntry : Entry
{
    internal ZipEntry(ZipArchiveFile fsFile) : base(fsFile)
    {
    }

    internal override byte[] Read()
    {
        var buff = MemoryUtils.ReadBytes(GetArchiveFile().Br, (long) Offset, (int) CompressedSize);
        return IsCompressed() ? Inflate(buff) : buff;
    }

    protected override byte[] Inflate(byte[] buff)
    {
        var inflatedBytes = new byte[Size];
        using var ms = new MemoryStream(buff);
        using var ds = new DeflateStream(ms, CompressionMode.Decompress);

        ds.Read(inflatedBytes, 0, (int) Size);

        return inflatedBytes;
    }

    internal override bool IsDirectory()
    {
        return CompressedSize == 0;
    }

    internal override bool IsCompressed()
    {
        return CompressedSize != Size;
    }
}
