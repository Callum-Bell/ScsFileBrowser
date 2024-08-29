using ScsFileBrowser.Utils;

namespace ScsFileBrowser.UI.Components;

public sealed class PathSelectorFlowLayoutPanel : FlowLayoutPanel
{
    private string _currentPath = "/";

    private bool _editMode;

    public PathSelectorFlowLayoutPanel()
    {
        AddPathPart("/", 0);
        var contextMenu = new ContextMenuStrip
        {
            Renderer = new ThemedToolStripMenuRenderer(),
            ShowImageMargin = false,
            BackColor = ThemedColors.Base
        };

        var copyPathMenuItem = new ThemedToolStripMenuItem
        {
            Text = "Copy Path"
        };
        copyPathMenuItem.Click += (_, _) => CommonUtils.TextToClipboard($"/{_currentPath}");
        contextMenu.Items.Add(copyPathMenuItem);

        var editPathMenuItem = new ThemedToolStripMenuItem
        {
            Text = "Edit Path"
        };
        editPathMenuItem.Click += (_, _) =>
        {
            _editMode = !_editMode;
            SetPath(_currentPath);
        };
        contextMenu.Items.Add(editPathMenuItem);

        ContextMenuStrip = contextMenu;
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        BackColor = ThemedColors.Overlay;
        Cursor = Cursors.IBeam;
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        BackColor = ThemedColors.Surface;
        Cursor = Cursors.Default;
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right)
        {
            _editMode = true;
            SetPath(_currentPath);
            return;
        }

        ContextMenuStrip.Show(e.Location);
    }

    internal event EventHandler<PathChangedEventArgs>? PathChanged;

    private void OnClickedPath(int index)
    {
        var pathParts = _currentPath.Split('/');
        if (index >= pathParts.Length) return;
        var selectedPath = string.Join("/", pathParts[..index]);
        PathChanged?.Invoke(this, new PathChangedEventArgs
        {
            NewPath = selectedPath
        });
    }

    private void AddPathPart(string pathPart, int index)
    {
        var b = new ThemedButton
        {
            Text = pathPart,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Bottom,
            FlatAppearance = {BorderSize = 0},
            Margin = new Padding(0),
            Cursor = Cursors.Hand
        };

        b.MouseClick += (s, e) => { OnClickedPath(index); };
        Controls.Add(b);
    }

    private void AddSeparator()
    {
        Controls.Add(new Label
        {
            Text = ">",
            AutoSize = true,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Margin = new Padding(0)
        });
    }

    private void AddInputUi()
    {
        using var f = new Font("Segoe UI", 11);
        var inputBox = new ThemedTextBox
        {
            Font = f,
            Dock = DockStyle.Fill,
            Text = _currentPath,
            Width = Width - 50
        };
        inputBox.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape)
            {
                _editMode = false;
                SetPath(_currentPath);
                return;
            }

            if (e.KeyCode != Keys.Enter) return;

            var eventArgs = new PathChangedEventArgs
            {
                NewPath = PathUtils.EnsureLocalPath(inputBox.Text)
            };
            _editMode = false;
            PathChanged?.Invoke(this, eventArgs);
        };
        inputBox.Leave += (_, _) =>
        {
            if (_editMode == false) return;
            _editMode = false;
            SetPath(_currentPath);
        };
        Controls.Add(inputBox);
        inputBox.Focus();
    }

    internal void SetPath(string path)
    {
        _currentPath = path;
        for (var i = Controls.Count - 1; i >= 1; i--) Controls[i].Dispose();

        if (_editMode)
            AddInputUi();
        else
        {
            var pathParts = _currentPath.Split("/", StringSplitOptions.RemoveEmptyEntries);

            if (pathParts.Length > 0) AddSeparator();

            for (var i = 0; i < pathParts.Length; i++)
            {
                AddPathPart(pathParts[i], i + 1);
                if (i < pathParts.Length - 1) AddSeparator();
            }
        }
    }

    internal bool InEditMode()
    {
        return _editMode;
    }

    internal class PathChangedEventArgs : EventArgs
    {
        public string NewPath { get; init; } = "";
    }
}
