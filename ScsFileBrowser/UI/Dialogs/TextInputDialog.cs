namespace ScsFileBrowser.UI.Dialogs;

public sealed partial class TextInputDialog : Form
{
    public TextInputDialog(string dialogTitle, string textInputLabel, string? defaultTextValue = null)
    {
        InitializeComponent();
        SetTheme();
        AcceptButton = ContinueBtn;
        CancelButton = CancelBtn;
        Text = dialogTitle;
        TextInputLabel.Text = textInputLabel;
        if (!string.IsNullOrWhiteSpace(defaultTextValue))
        {
            InputTextBox.Text = defaultTextValue;
            TextValue = defaultTextValue;
        }
    }

    public string TextValue { get; private set; } = "";

    private void SetTheme()
    {
        BackColor = ThemedColors.Surface;
        ForeColor = ThemedColors.Text;
    }

    private void InputTextBox_TextChanged(object sender, EventArgs e)
    {
        TextValue = InputTextBox.Text;
    }

    private void InputTextBox_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        else if (e.KeyCode == Keys.Enter)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
