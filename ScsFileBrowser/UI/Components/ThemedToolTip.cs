using System.ComponentModel;

namespace ScsFileBrowser.UI.Components;

public class ThemedToolTip : ToolTip
{
    public ThemedToolTip()
    {
        SetTheme();
    }

    public ThemedToolTip(IContainer cont) : base(cont)
    {
        SetTheme();
    }

    private void SetTheme()
    {
        Draw += (_, e) =>
        {
            e.DrawBackground();
            using var borderPen = new Pen(ThemedColors.Love);
            e.Graphics.DrawRectangle(borderPen, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
            using var textBrush = new SolidBrush(ThemedColors.Text);
            e.Graphics.DrawString(e.ToolTipText, e.Font, textBrush, new PointF(0, 0), StringFormat.GenericDefault);
        };
    }
}
