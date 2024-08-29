namespace ScsFileBrowser.UI.Components;

public class ThemedToolStripMenuRenderer : ToolStripProfessionalRenderer
{
    public ThemedToolStripMenuRenderer() : base(new ThemedToolStripColors())
    {
    }

    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
        if (e.Item is ToolStripMenuItem tsMenuItem)
            e.ArrowColor = ThemedColors.Text;
        base.OnRenderArrow(e);
    }
}

public class ThemedToolStripColors : ProfessionalColorTable
{
    public override Color MenuItemSelected => ThemedColors.HighlightMed;
    public override Color MenuItemSelectedGradientBegin => ThemedColors.HighlightMed;
    public override Color MenuItemSelectedGradientEnd => ThemedColors.HighlightMed;
    public override Color MenuStripGradientBegin => ThemedColors.Base;
    public override Color MenuStripGradientEnd => ThemedColors.Base;
    public override Color MenuItemBorder => Color.Transparent;
    public override Color ToolStripBorder => Color.Transparent;
    public override Color MenuBorder => ThemedColors.Love;
}
