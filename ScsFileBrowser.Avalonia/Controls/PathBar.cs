using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using ScsFileBrowser.AvaloniaUI.Services;
using ScsFileBrowser.Utils;

namespace ScsFileBrowser.AvaloniaUI.Controls;

public sealed class PathChangedEventArgs : EventArgs
{
    public string NewPath { get; init; } = "";
}

/// <summary>
///     Breadcrumb style path selector, click a part of the path to go to it, click the empty space to edit the path as text
/// </summary>
public sealed class PathBar : UserControl
{
    private readonly Border _border;
    private readonly StackPanel _panel;
    private string _currentPath = "";
    private bool _editMode;

    public PathBar()
    {
        _panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center
        };

        _border = new Border
        {
            MinHeight = 30,
            Padding = new Thickness(2),
            Cursor = new Cursor(StandardCursorType.Ibeam),
            Child = _panel
        };
        _border.Classes.Add("pathbar");
        _border.PointerReleased += Border_PointerReleased;

        Content = _border;
        SetPath("");
    }

    public bool IsEditing => _editMode;

    public event EventHandler<PathChangedEventArgs>? PathChanged;

    public void SetPath(string path)
    {
        _currentPath = path;
        _panel.Children.Clear();

        AddPathPart("/", 0);

        if (_editMode)
        {
            AddInputBox();
            return;
        }

        var pathParts = _currentPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (pathParts.Length > 0) AddSeparator();

        for (var i = 0; i < pathParts.Length; i++)
        {
            AddPathPart(pathParts[i], i + 1);
            if (i < pathParts.Length - 1) AddSeparator();
        }
    }

    private void OnClickedPath(int index)
    {
        var pathParts = _currentPath.Split('/');
        if (index >= pathParts.Length) return;

        PathChanged?.Invoke(this, new PathChangedEventArgs
        {
            NewPath = string.Join("/", pathParts[..index])
        });
    }

    private void AddPathPart(string pathPart, int index)
    {
        // A TextBlock is used as content so '_' in names is not treated as an access key marker
        var button = new Button
        {
            Content = new TextBlock {Text = pathPart, FontSize = 14},
            Cursor = new Cursor(StandardCursorType.Hand)
        };
        button.Classes.Add("pathpart");
        button.Click += (_, _) => OnClickedPath(index);
        _panel.Children.Add(button);
    }

    private void AddSeparator()
    {
        var separator = new TextBlock
        {
            Text = ">",
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(2, 0)
        };
        separator.Classes.Add("pathsep");
        _panel.Children.Add(separator);
    }

    private void AddInputBox()
    {
        var inputBox = new TextBox
        {
            Text = _currentPath,
            Width = Math.Max(200, Bounds.Width - 60),
            VerticalAlignment = VerticalAlignment.Center
        };

        inputBox.KeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                _editMode = false;
                SetPath(_currentPath);
                return;
            }

            if (e.Key != Key.Enter) return;

            e.Handled = true;
            var eventArgs = new PathChangedEventArgs
            {
                NewPath = PathUtils.EnsureLocalPath(inputBox.Text ?? "")
            };
            _editMode = false;
            PathChanged?.Invoke(this, eventArgs);
            // Listeners normally call SetPath with the new path, this makes sure the text box is gone either way
            SetPath(_currentPath);
        };

        inputBox.LostFocus += (_, _) =>
        {
            if (!_editMode) return;
            _editMode = false;
            SetPath(_currentPath);
        };

        _panel.Children.Add(inputBox);

        // Focusing has to wait until the text box is part of the visual tree
        Dispatcher.UIThread.Post(() =>
        {
            inputBox.Focus();
            inputBox.SelectAll();
        }, DispatcherPriority.Input);
    }

    private void Border_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        // Clicks on the path part buttons are handled (and marked as handled) by the buttons themselves
        if (e.InitialPressMouseButton == MouseButton.Right)
        {
            e.Handled = true;
            ShowContextMenu();
            return;
        }

        if (e.InitialPressMouseButton != MouseButton.Left || _editMode) return;

        e.Handled = true;
        _editMode = true;
        SetPath(_currentPath);
    }

    private void ShowContextMenu()
    {
        var contextMenu = new ContextMenu();

        var copyPathMenuItem = new MenuItem {Header = "Copy Path"};
        copyPathMenuItem.Click += async (_, _) => await ClipboardService.SetTextAsync(this, $"/{_currentPath}");
        contextMenu.Items.Add(copyPathMenuItem);

        var editPathMenuItem = new MenuItem {Header = "Edit Path"};
        editPathMenuItem.Click += (_, _) =>
        {
            _editMode = !_editMode;
            SetPath(_currentPath);
        };
        contextMenu.Items.Add(editPathMenuItem);

        contextMenu.Open(_border);
    }
}
