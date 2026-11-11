using Avalonia;
using Avalonia.Controls;
using ScsFileBrowser.Logging;

namespace ScsFileBrowser.AvaloniaUI.Services;

internal static class ClipboardService
{
    /// <summary>
    ///     Puts <paramref name="text" /> on the clipboard of the window that contains <paramref name="visual" />
    /// </summary>
    internal static async Task SetTextAsync(Visual visual, string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        var clipboard = TopLevel.GetTopLevel(visual)?.Clipboard;
        if (clipboard == null)
        {
            Logger.Instance.Error("Could not access the clipboard");
            return;
        }

        await clipboard.SetTextAsync(text);
    }
}
