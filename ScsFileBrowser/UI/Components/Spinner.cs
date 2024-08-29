using Timer = System.Threading.Timer;

namespace ScsFileBrowser.UI.Components;

public class Spinner : Control
{
    private readonly Pen _spinnerPen = new(ThemedColors.Gold, 2);
    private float _angle;
    private Timer? _timer;

    public Spinner()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint,
            true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        const float spinnerSize = 20f;

        if ((_angle += 3) >= 360) _angle %= 360;

        e.Graphics.DrawArc(_spinnerPen, 0, 0, spinnerSize,
            spinnerSize, _angle, 120);
    }

    internal void Start()
    {
        _timer ??= new Timer(_ => { Invalidate(); }, null, 0, 10);
    }

    internal void Stop()
    {
        _timer?.Dispose();
        _timer = null;
    }
}
