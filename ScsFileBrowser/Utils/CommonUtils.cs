using ScsFileBrowser.FileSystem;

namespace ScsFileBrowser.Utils;

internal static class CommonUtils
{
    internal static string FormatSize(ulong size)
    {
        if (size >= 1024)
        {
            var kb = Math.Round(size / 1024f);
            return $"{kb:#,##0} KB";
        }

        return $"{size} B";
    }

    internal static void TextToClipboard(string path)
    {
        if (path == "") return;
        Clipboard.SetText(path);
    }

    /// <summary>
    ///     Extracts the data from the <paramref name="file" /> to <paramref name="extractionDir" /> with name/path
    ///     <paramref name="customFileName" /> or a slugged version of the <paramref name="originalPath" />
    /// </summary>
    /// <param name="file">The file to extract</param>
    /// <param name="extractionDir">Directory to extract the file to</param>
    /// <param name="originalPath">Original path of the file in the archive</param>
    /// <param name="customFileName">If not provided/null it will use the path as the filename and change all '/' to '+'</param>
    /// <returns>The path of the extracted file</returns>
    /// <exception cref="InvalidDataException"></exception>
    internal static string ExtractFile(UberFile file, string extractionDir, string originalPath,
        string? customFileName = null)
    {
        return ExtractFile(file.Entry, extractionDir, originalPath, customFileName);
    }

    /// <summary>
    ///     Extracts the data from <paramref name="entry" /> to <paramref name="extractionDir" /> with name/path
    ///     <paramref name="customFileName" /> or a slugged version of the <paramref name="originalPath" />
    /// </summary>
    /// <param name="entry">The entry of a file to extract</param>
    /// <param name="extractionDir">Directory to extract the file to</param>
    /// <param name="originalPath">Original path of the file in the archive</param>
    /// <param name="customFileName">If not provided/null it will use the path as the filename and change all '/' to '+'</param>
    /// <returns>The path of the extracted file</returns>
    /// <exception cref="InvalidDataException"></exception>
    internal static string ExtractFile(Entry entry, string extractionDir, string originalPath,
        string? customFileName = null)
    {
        var fileName = string.IsNullOrWhiteSpace(customFileName) ? originalPath.Replace('/', '+') : customFileName;

        if (entry.Size <= 0 || entry.CompressedSize <= 0) throw new InvalidDataException("The file entry was empty");

        var path = Path.Combine(extractionDir, fileName);
        var folderName = Path.GetDirectoryName(path);
        if (folderName == null) throw new InvalidDataException($"Could not get directory for '{path}'");
        Directory.CreateDirectory(folderName);
        var bytes = entry.Read();
        var magic = bytes.Length >= 4 ? MemoryUtils.ReadUInt32(bytes, 0) : 0;

        File.WriteAllBytes(path, magic == 21720627 ? MemoryUtils.Decrypt3Nk(bytes) : bytes);
        return path;
    }
}
