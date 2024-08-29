namespace ScsFileBrowser.UI.Components;

public sealed class ThemedToolStripMenuItem : ToolStripMenuItem
{
    public ThemedToolStripMenuItem()
    {
        if (DesignMode)
            return;
        BackColor = ThemedColors.Base;
        ForeColor = ThemedColors.Text;
    }
}
