namespace ScsFileBrowser.UI.Dialogs;

public sealed partial class ComboTextInputDialog : Form
{
    public ComboTextInputDialog(string dialogTitle, string textInputLabel, string comboInputLabel,
        List<string> comboOptions,
        string? defaultTextValue = null, string? defaultComboValue = null)
    {
        InitializeComponent();
        SetTheme();
        AcceptButton = ContinueBtn;
        CancelButton = CancelBtn;
        Text = dialogTitle;
        TextInputLabel.Text = textInputLabel;
        ComboBoxInputLabel.Text = comboInputLabel;
        InputComboBox.DataSource = comboOptions;
        if (!string.IsNullOrWhiteSpace(defaultTextValue))
        {
            InputTextBox.Text = defaultTextValue;
            TextValue = defaultTextValue;
        }

        if (!string.IsNullOrWhiteSpace(defaultComboValue)) InputComboBox.SelectedItem = defaultComboValue;
        ComboValue = InputComboBox.SelectedItem as string;
    }

    public string? ComboValue { get; private set; }
    public string TextValue { get; private set; } = "";

    private void SetTheme()
    {
        BackColor = ThemedColors.Surface;
        ForeColor = ThemedColors.Text;

        InputComboBox.BackColor = ThemedColors.Overlay;
        InputComboBox.ForeColor = ThemedColors.Text;
    }

    private void InputTextBox_TextChanged(object sender, EventArgs e)
    {
        TextValue = InputTextBox.Text;
    }

    private void InputComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        ComboValue = InputComboBox.SelectedItem as string;
    }
}
