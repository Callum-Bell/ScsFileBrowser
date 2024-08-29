using ScsFileBrowser.Utils;

namespace ScsFileBrowser.FileSystem;

/// <summary>
///     Contains the data of an entry in an <see cref="ArchiveFile" />.
///     Has location and size of the raw data of either a file or a directory
/// </summary>
internal abstract class Entry
{
    private readonly ArchiveFile _archiveFile;

    internal Entry(ArchiveFile fsFile)
    {
        _archiveFile = fsFile;
    }

    /// <summary>
    ///     Offset in file to where the data starts
    /// </summary>
    internal ulong Offset { get; set; }

    /// <summary>
    ///     FileModel/dir path hashed by CityHash64
    /// </summary>
    internal ulong Hash { get; set; }

    /// <summary>
    ///     Total size when inflated
    /// </summary>
    internal uint Size { get; set; }

    /// <summary>
    ///     Size in archive file
    /// </summary>
    internal uint CompressedSize { get; set; }

    /// <summary>
    ///     Get's the <see cref="ArchiveFile" /> (.scs) where the current entry is located in.
    /// </summary>
    internal ArchiveFile GetArchiveFile()
    {
        return _archiveFile;
    }

    /// <summary>
    ///     Reads the entry data from the archive file
    /// </summary>
    /// <returns>The data, inflating it if necessary.</returns>
    internal abstract byte[] Read();

    /// <summary>
    ///     Inflates the compressed data
    /// </summary>
    /// <returns>Inflated data</returns>
    protected abstract byte[] Inflate(byte[] buff);


    /// <returns>Given hash for hash files, CityHashed path for zip files</returns>
    internal ulong GetHash()
    {
        return Hash;
    }

    internal string GetFormattedSize()
    {
        return CommonUtils.FormatSize(Size);
    }

    internal abstract bool IsCompressed();

    internal abstract bool IsDirectory();
}
