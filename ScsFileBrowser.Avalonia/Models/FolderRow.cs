namespace ScsFileBrowser.AvaloniaUI.Models;

/// <summary>
///     A row in the folder list
/// </summary>
public sealed class FolderRow
{
    public string Name { get; init; } = "";
    public int SubEntryCount { get; init; }
}
