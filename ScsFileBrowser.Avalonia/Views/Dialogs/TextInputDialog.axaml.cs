using Avalonia.Controls;
using Avalonia.Input;

namespace ScsFileBrowser.AvaloniaUI.Views.Dialogs;

public partial class TextInputDialog : Window
{
    public TextInputDialog()
    {
        InitializeComponent();
    }

    private TextInputDialog(string dialogTitle, string textInputLabel, string? defaultTextValue) : this()
    {
        Title = dialogTitle;
        LabelText.Text = textInputLabel;
        if (!string.IsNullOrWhiteSpace(defaultTextValue)) InputBox.Text = defaultTextValue;

        ContinueButton.Click += (_, _) => Close(true);
        CancelButton.Click += (_, _) => Close(false);
        InputBox.KeyDown += InputBox_KeyDown;
        Opened += (_, _) =>
        {
            InputBox.Focus();
            InputBox.SelectAll();
        };
    }

    private void InputBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            Close(false);
        }
        else if (e.Key == Key.Enter)
        {
            e.Handled = true;
            Close(true);
        }
    }

    /// <returns>The entered text, null if the dialog was cancelled</returns>
    internal static async Task<string?> ShowAsync(Window owner, string dialogTitle, string textInputLabel,
        string? defaultTextValue = null)
    {
        var dialog = new TextInputDialog(dialogTitle, textInputLabel, defaultTextValue);
        var accepted = await dialog.ShowDialog<bool>(owner);
        return accepted ? dialog.InputBox.Text ?? "" : null;
    }
}
