namespace ScsFileBrowser.UI.Components;

public class ThemedDropButton : ThemedButton
{
    public ThemedDropButton()
    {
        SetStyle(ControlStyles.Selectable, false);
        FlatAppearance.MouseOverBackColor = ThemedColors.HighlightMed;
    }

    internal ContextMenuStrip? Menu { get; set; }

    internal bool ShowMenuUnderCursor { get; set; }

    protected override void OnMouseDown(MouseEventArgs mevent)
    {
        base.OnMouseDown(mevent);

        if (Menu == null || mevent.Button != MouseButtons.Left) return;
        var menuLocation = ShowMenuUnderCursor ? mevent.Location : new Point(0, Height);

        Menu.Show(this, menuLocation);
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        base.OnPaint(pevent);

        if (Menu == null) return;

        var arrowX = ClientRectangle.Width - 14;
        var arrowY = ClientRectangle.Height / 2 - 1;

        using var brush = new SolidBrush(ThemedColors.Text);
        Point[] arrows = {new(arrowX, arrowY), new(arrowX + 7, arrowY), new(arrowX + 3, arrowY + 4)};
        pevent.Graphics.FillPolygon(new SolidBrush(ThemedColors.Text), arrows);
    }
}
