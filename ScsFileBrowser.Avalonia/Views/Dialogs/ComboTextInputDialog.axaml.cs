using Avalonia.Controls;
using Avalonia.Input;

namespace ScsFileBrowser.AvaloniaUI.Views.Dialogs;

public sealed record ComboInputResult(string Text, string Combo);

public partial class ComboTextInputDialog : Window
{
    public ComboTextInputDialog()
    {
        InitializeComponent();
    }

    private ComboTextInputDialog(string dialogTitle, string textInputLabel, string comboInputLabel,
        List<string> comboOptions, string? defaultTextValue, string? defaultComboValue) : this()
    {
        Title = dialogTitle;
        TextLabel.Text = textInputLabel;
        ComboLabel.Text = comboInputLabel;

        InputCombo.ItemsSource = comboOptions;
        InputCombo.SelectedIndex = comboOptions.Count > 0 ? 0 : -1;
        if (!string.IsNullOrWhiteSpace(defaultComboValue) && comboOptions.Contains(defaultComboValue))
            InputCombo.SelectedItem = defaultComboValue;

        if (!string.IsNullOrWhiteSpace(defaultTextValue)) InputBox.Text = defaultTextValue;

        ContinueButton.Click += (_, _) => Close(true);
        CancelButton.Click += (_, _) => Close(false);
        InputBox.KeyDown += InputBox_KeyDown;
        Opened += (_, _) => InputBox.Focus();
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

    /// <returns>The entered text and selected option, null if the dialog was cancelled</returns>
    internal static async Task<ComboInputResult?> ShowAsync(Window owner, string dialogTitle, string textInputLabel,
        string comboInputLabel, List<string> comboOptions, string? defaultTextValue = null,
        string? defaultComboValue = null)
    {
        var dialog = new ComboTextInputDialog(dialogTitle, textInputLabel, comboInputLabel, comboOptions,
            defaultTextValue, defaultComboValue);
        var accepted = await dialog.ShowDialog<bool>(owner);
        if (!accepted) return null;

        return new ComboInputResult(dialog.InputBox.Text ?? "", dialog.InputCombo.SelectedItem as string ?? "");
    }
}
