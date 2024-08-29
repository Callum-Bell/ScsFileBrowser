using ScsFileBrowser.FileSystem.Hash;
using ScsFileBrowser.FileSystem.Zip;
using ScsFileBrowser.Logging;
using ScsFileBrowser.Utils;

namespace ScsFileBrowser.FileSystem;

internal class UberFileSystem
{
    private static readonly Lazy<UberFileSystem> _instance = new(() => new UberFileSystem());
    private readonly List<ArchiveFile> _archiveFiles = new();
    internal static UberFileSystem Instance => _instance.Value;

    /// <summary>
    ///     Reads and adds a single file to the filesystem
    ///     Checks if file is an SCS Hash file, if not, assumes it's a zip file
    /// </summary>
    /// <param name="path">Path for the archive file to add</param>
    /// <returns>Whether or not the file was parsed correctly</returns>
    internal bool AddSourceFile(string path)
    {
        if (!File.Exists(path))
        {
            Logger.Instance.Error($"Could not find file '{path}'");
            return false;
        }

        var buff = new byte[4];
        using (var f = File.OpenRead(path))
        {
            f.Seek(0, SeekOrigin.Begin);
            f.Read(buff, 0, 4); // read magic bytes (first 4 bytes of file)
        }

        if (MemoryUtils.ReadUInt32(buff, 0) == Consts.ScsMagic)
        {
            var hashFile = new HashArchiveFile(path);
            if (hashFile.Parse())
            {
                _archiveFiles.Add(hashFile);
                return true;
            }

            Logger.Instance.Error($"Could not load hash file '{path}'");
            return false;
        }

        var zipFile = new ZipArchiveFile(path);
        if (zipFile.Parse())
        {
            _archiveFiles.Add(zipFile);
            return true;
        }

        Logger.Instance.Error($"Could not load zip file '{path}'");
        return false;
    }

    /// <summary>
    ///     Adds all files from the specified directory matching the filter to the filesystem
    /// </summary>
    /// <param name="path">Path to the directory where to find the files to include</param>
    /// <param name="searchPattern">Search pattern to select specific files eg. "*.scs"</param>
    /// <returns>Whether or not all files were added successfully</returns>
    internal bool AddSourceDirectory(string path, string searchPattern = "*.scs")
    {
        if (!Directory.Exists(path))
        {
            Logger.Instance.Error($"Could not find directory '{path}'");
            return false;
        }

        var scsFilesPaths = Directory.GetFiles(path, searchPattern);

        var result = true;

        foreach (var scsFilePath in scsFilesPaths)
        {
            var fileResult = AddSourceFile(scsFilePath);
            if (!fileResult) result = false;
        }

        return result;
    }

    /// <summary>
    ///     Tries to find the directory by the given path
    /// </summary>
    /// <param name="path">Path for the wanted directory</param>
    /// <returns>
    ///     <see cref="UberDirectory" /> for the given path
    ///     <para>Null if path was not found</para>
    /// </returns>
    internal IReadOnlyList<UberDirectory> GetDirectoryEntries(string path)
    {
        var dirs = new List<UberDirectory>();
        foreach (var archiveFile in _archiveFiles)
        {
            var af = archiveFile.GetDirectory(path);
            if (af != null) dirs.Add(af);
        }

        return dirs;
    }

    /// <summary>
    ///     Tries to find the directory by the given hash
    /// </summary>
    /// <param name="pathHash">Hash for the wanted directory</param>
    /// <returns>
    ///     <see cref="UberDirectory" /> for the given hash
    ///     <para>Null if hash was not found</para>
    /// </returns>
    internal IReadOnlyList<UberDirectory> GetDirectoryEntries(ulong pathHash)
    {
        var dirs = new List<UberDirectory>();
        foreach (var archiveFile in _archiveFiles)
        {
            var af = archiveFile.GetDirectory(pathHash);
            if (af != null) dirs.Add(af);
        }

        return dirs;
    }

    /// <summary>
    ///     Tries to find the file by a given path
    /// </summary>
    /// <param name="path">Path for the wanted file</param>
    /// <returns>
    ///     <see cref="UberFile" /> for the given path
    ///     <para>Null if path was not found</para>
    /// </returns>
    internal IReadOnlyList<UberFile> GetFileEntries(string path)
    {
        var files = new List<UberFile>();
        foreach (var archiveFile in _archiveFiles)
        {
            var af = archiveFile.GetFile(path);
            if (af != null) files.Add(af);
        }

        return files;
    }

    /// <summary>
    ///     Tries to find the file by a given hash
    /// </summary>
    /// <param name="pathHash">Hash for the wanted file</param>
    /// <returns>
    ///     <see cref="UberFile" /> for the given hash
    ///     <para>Null if hash was not found</para>
    /// </returns>
    internal IReadOnlyList<UberFile> GetFileEntries(ulong pathHash)
    {
        var files = new List<UberFile>();
        foreach (var archiveFile in _archiveFiles)
        {
            var af = archiveFile.GetFile(pathHash);
            if (af != null) files.Add(af);
        }

        return files;
    }

    internal void Reset()
    {
        _archiveFiles.ForEach(a => a.Dispose());
        _archiveFiles.Clear();
    }
}
