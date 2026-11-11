using ScsFileBrowser.Utils;

namespace ScsFileBrowser.AvaloniaUI.Models;

/// <summary>
///     A row in the file list
/// </summary>
public sealed class FileRow
{
    public string Name { get; init; } = "";

    /// <summary>
    ///     Name of the archive the file comes from (with "(+n)" appended when more archives contain the file)
    /// </summary>
    public string Source { get; init; } = "";

    public uint Size { get; init; }

    public string SizeText => CommonUtils.FormatSize(Size);

    /// <summary>
    ///     Names of all the archives containing the file, only set when there is more than one
    /// </summary>
    public string? SourcesTooltip { get; init; }
}
