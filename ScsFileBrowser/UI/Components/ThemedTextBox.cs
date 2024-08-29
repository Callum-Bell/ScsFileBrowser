namespace ScsFileBrowser.UI.Components;

public sealed class ThemedTextBox : TextBox
{
    public ThemedTextBox()
    {
        BorderStyle = BorderStyle.None;
        BackColor = ThemedColors.Overlay;
        ForeColor = ThemedColors.Text;
        Padding = new Padding(2, 5, 2, 5);
    }
}
