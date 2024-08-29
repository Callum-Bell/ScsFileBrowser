namespace ScsFileBrowser.UI.Components;

public class ThemedButton : Button
{
    public ThemedButton()
    {
        FlatStyle = FlatStyle.Flat;
        SetStyle(ControlStyles.Selectable, false);
        FlatAppearance.MouseOverBackColor = ThemedColors.HighlightMed;
        FlatAppearance.BorderColor = ThemedColors.HighlightMed;
    }
}
