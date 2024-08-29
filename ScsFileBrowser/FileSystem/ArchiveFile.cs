using ScsFileBrowser.Utils;

namespace ScsFileBrowser.FileSystem;

/// <summary>
///     Represents an archive file (*.scs, *.zip)
/// </summary>
internal abstract class ArchiveFile : IDisposable
{
    protected readonly string Path;

    internal ArchiveFile(string path)
    {
        Path = path;
    }

    internal Dictionary<ulong, UberFile> Files { get; } = new();
    internal Dictionary<ulong, UberDirectory> Directories { get; } = new();
    internal Dictionary<ulong, Entry> Entries { get; } = new();

    protected internal BinaryReader Br { get; protected set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    internal abstract bool Parse();

    internal string GetPath()
    {
        return Path;
    }

    internal string GetName()
    {
        return System.IO.Path.GetFileNameWithoutExtension(GetPath());
    }

    ~ArchiveFile()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (disposing) Br.Dispose();
    }

    /// <summary>
    ///     Tries to find the directory by the given path
    /// </summary>
    /// <param name="path">Path for the wanted directory</param>
    /// <returns>
    ///     <see cref="UberDirectory" /> for the given path
    ///     <para>Null if path was not found</para>
    /// </returns>
    internal virtual UberDirectory? GetDirectory(string path)
    {
        return GetDirectory(CityHash.CityHash64(PathUtils.EnsureLocalPath(path)));
    }

    /// <summary>
    ///     Tries to find the directory by the given hash
    /// </summary>
    /// <param name="pathHash">Hash for the wanted directory</param>
    /// <returns>
    ///     <see cref="UberDirectory" /> for the given hash
    ///     <para>Null if hash was not found</para>
    /// </returns>
    internal UberDirectory? GetDirectory(ulong pathHash)
    {
        return Directories.ContainsKey(pathHash) ? Directories[pathHash] : null;
    }

    /// <summary>
    ///     Tries to find the file by a given path
    /// </summary>
    /// <param name="path">Path for the wanted file</param>
    /// <returns>
    ///     <see cref="UberFile" /> for the given path
    ///     <para>Null if path was not found</para>
    /// </returns>
    internal virtual UberFile? GetFile(string path)
    {
        return GetFile(CityHash.CityHash64(PathUtils.EnsureLocalPath(path)));
    }

    /// <summary>
    ///     Tries to find the file by a given hash
    /// </summary>
    /// <param name="pathHash">Hash for the wanted file</param>
    /// <returns>
    ///     <see cref="UberFile" /> for the given hash
    ///     <para>Null if hash was not found</para>
    /// </returns>
    internal UberFile? GetFile(ulong pathHash)
    {
        return Files.ContainsKey(pathHash) ? Files[pathHash] : null;
    }
}
