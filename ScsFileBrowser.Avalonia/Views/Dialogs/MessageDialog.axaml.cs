using Avalonia.Controls;
using Avalonia.Layout;

namespace ScsFileBrowser.AvaloniaUI.Views.Dialogs;

public enum MessageButtons
{
    Ok,
    YesNo,
    YesNoCancel
}

public enum MessageResult
{
    /// <summary>
    ///     The dialog was closed without pressing one of the buttons (e.g. with the window close button)
    /// </summary>
    None,
    Ok,
    Yes,
    No,
    Cancel
}

/// <summary>
///     Replacement for the WinForms MessageBox
/// </summary>
public partial class MessageDialog : Window
{
    public MessageDialog()
    {
        InitializeComponent();
    }

    private MessageDialog(string message, string title, MessageButtons buttons) : this()
    {
        Title = title;
        MessageText.Text = message;

        switch (buttons)
        {
            case MessageButtons.Ok:
                AddButton("OK", MessageResult.Ok, true, true);
                break;
            case MessageButtons.YesNo:
                AddButton("Yes", MessageResult.Yes, true);
                AddButton("No", MessageResult.No, false, true);
                break;
            case MessageButtons.YesNoCancel:
                AddButton("Yes", MessageResult.Yes, true);
                AddButton("No", MessageResult.No);
                AddButton("Cancel", MessageResult.Cancel, false, true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null);
        }
    }

    private void AddButton(string text, MessageResult result, bool isDefault = false, bool isCancel = false)
    {
        var button = new Button
        {
            Content = text,
            MinWidth = 80,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            IsDefault = isDefault,
            IsCancel = isCancel
        };
        button.Click += (_, _) => Close(result);
        ButtonPanel.Children.Add(button);
    }

    internal static async Task<MessageResult> ShowAsync(Window owner, string message,
        string title = "ScsFileBrowser", MessageButtons buttons = MessageButtons.Ok)
    {
        var dialog = new MessageDialog(message, title, buttons);
        return await dialog.ShowDialog<MessageResult>(owner);
    }
}
