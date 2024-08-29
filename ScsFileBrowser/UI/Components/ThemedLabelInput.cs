namespace ScsFileBrowser.UI.Components;

public sealed class ThemedLabelInput : TextBox
{
    public ThemedLabelInput()
    {
        BorderStyle = BorderStyle.None;
        BackColor = ThemedColors.Surface;
        ForeColor = ThemedColors.Text;
        Padding = new Padding(2, 5, 2, 5);
    }
}
