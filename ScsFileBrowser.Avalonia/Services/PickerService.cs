using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ScsFileBrowser.Logging;

namespace ScsFileBrowser.AvaloniaUI.Services;

/// <summary>
///     Wrappers around the file/folder pickers that work with plain paths
/// </summary>
internal static class PickerService
{
    internal static IReadOnlyList<FilePickerFileType> ArchiveFileTypes { get; } = new[]
    {
        new FilePickerFileType("SCS archives")
        {
            Patterns = Consts.AllowedDropExtensions.Select(e => $"*{e}").ToArray()
        },
        FilePickerFileTypes.All
    };

    /// <param name="startPath">File or folder whose directory the picker should start in</param>
    /// <returns>The selected path, null if the picker was cancelled</returns>
    internal static async Task<string?> PickFileAsync(TopLevel owner, string title, string? startPath,
        IReadOnlyList<FilePickerFileType>? fileTypes = null)
    {
        try
        {
            var options = new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                FileTypeFilter = fileTypes,
                SuggestedStartLocation = await GetFolderAsync(owner, GetStartDirectory(startPath))
            };

            var files = await owner.StorageProvider.OpenFilePickerAsync(options);
            return files.Count == 0 ? null : files[0].TryGetLocalPath();
        }
        catch (Exception e)
        {
            Logger.Instance.Error($"Could not show the file picker: {e}");
            return null;
        }
    }

    /// <returns>The selected folder path, null if the picker was cancelled</returns>
    internal static async Task<string?> PickFolderAsync(TopLevel owner, string title, string? startPath)
    {
        try
        {
            var options = new FolderPickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                SuggestedStartLocation = await GetFolderAsync(owner, startPath)
            };

            var folders = await owner.StorageProvider.OpenFolderPickerAsync(options);
            return folders.Count == 0 ? null : folders[0].TryGetLocalPath();
        }
        catch (Exception e)
        {
            Logger.Instance.Error($"Could not show the folder picker: {e}");
            return null;
        }
    }

    private static string? GetStartDirectory(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;
        return Directory.Exists(path) ? path : Path.GetDirectoryName(path);
    }

    private static async Task<IStorageFolder?> GetFolderAsync(TopLevel owner, string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path)) return null;

        try
        {
            return await owner.StorageProvider.TryGetFolderFromPathAsync(path);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
