namespace ScsFileBrowser.Utils;

internal static class PathUtils
{
    /// <summary>
    ///     Combines two paths together with forward slashes with the leading slash removed
    /// </summary>
    /// <param name="firstPath"></param>
    /// <param name="secondPath"></param>
    /// <returns>The combined path</returns>
    internal static string CombinePath(string firstPath, string secondPath)
    {
        var fullPath = Path.Combine(firstPath, EnsureLocalPath(secondPath));

        return EnsureLocalPath(fullPath.Replace('\\', '/'));
    }

    /// <summary>
    ///     Removes leading slash if the path contains it
    /// </summary>
    internal static string EnsureLocalPath(string path)
    {
        return path.StartsWith('/') ? path[1..] : path;
    }
}
