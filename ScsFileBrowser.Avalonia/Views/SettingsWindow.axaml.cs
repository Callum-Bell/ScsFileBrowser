using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using ScsFileBrowser.AvaloniaUI.Services;
using ScsFileBrowser.AvaloniaUI.Views.Dialogs;
using ScsFileBrowser.Logging;
using ScsFileBrowser.Settings;
using SettingsProgram = ScsFileBrowser.Settings.Program;

namespace ScsFileBrowser.AvaloniaUI.Views;

public partial class SettingsWindow : Window
{
    private const string HelpText = "- Drag an extension to a program to assign it to the program.\n" +
                                    "- Right click on an extension or program for options to rename or delete it.";

    /// <summary>
    ///     Distance (in pixels) the pointer has to move with the button down before a drag starts
    /// </summary>
    private const double DragThreshold = 8;

    // Dragged data, the names of the extensions separated by new lines
    private static readonly DataFormat<string> ExtensionsFormat =
        DataFormat.CreateStringApplicationFormat("scsfilebrowser-extensions");

    private bool _closeConfirmed;
    private Extension? _dragExtension;
    private Point? _dragStartPoint;
    private bool _hasChangesPending;

    public SettingsWindow()
    {
        InitializeComponent();

        ToolTip.SetTip(HelpButton, HelpText);
        HelpButton.Click += async (_, _) => await MessageDialog.ShowAsync(this, HelpText, "Settings Help");
        AddProgramButton.Click += async (_, _) => await AddProgramAsync();
        AddExtensionButton.Click += async (_, _) => await AddExtensionAsync();
        SaveButton.Click += (_, _) =>
        {
            _hasChangesPending = false;
            SettingsManager.Instance.Save();
        };

        SettingsTree.AddHandler(PointerPressedEvent, SettingsTree_PointerPressed, RoutingStrategies.Tunnel);
        SettingsTree.AddHandler(PointerMovedEvent, SettingsTree_PointerMoved, RoutingStrategies.Bubble, true);
        SettingsTree.AddHandler(PointerReleasedEvent, SettingsTree_PointerReleased, RoutingStrategies.Bubble, true);

        DragDrop.SetAllowDrop(SettingsTree, true);
        SettingsTree.AddHandler(DragDrop.DragOverEvent, SettingsTree_DragOver);
        SettingsTree.AddHandler(DragDrop.DragLeaveEvent, (_, _) => InfoLabel.Text = "");
        SettingsTree.AddHandler(DragDrop.DropEvent, SettingsTree_Drop);

        UpdateList();
    }

    #region Tree

    private void UpdateList()
    {
        var settings = SettingsManager.Instance.Settings;

        SettingsTree.Items.Clear();
        foreach (var program in settings.Programs.OrderBy(p => p.Name))
        {
            var programItem = new TreeViewItem {Header = program.Name, Tag = program, IsExpanded = true};

            foreach (var extension in settings.Extensions.Where(e => e.DefaultProgram == program.Name)
                         .OrderBy(e => e.Name))
                programItem.Items.Add(new TreeViewItem {Header = extension.Name, Tag = extension});

            SettingsTree.Items.Add(programItem);
        }

        EmptyLabel.IsVisible = settings.Programs.Count == 0;
    }

    private static TreeViewItem? GetTreeItem(RoutedEventArgs e)
    {
        return e.Source is Visual source ? source.FindAncestorOfType<TreeViewItem>(true) : null;
    }

    private SettingsProgram? GetSelectedProgram()
    {
        return (SettingsTree.SelectedItem as TreeViewItem)?.Tag as SettingsProgram;
    }

    #endregion

    #region Adding

    private async Task AddProgramAsync()
    {
        var programPath = await PickerService.PickFileAsync(this, "Select a program", null);
        if (programPath == null) return;

        var programName = Path.GetFileNameWithoutExtension(programPath);

        var inputName = await TextInputDialog.ShowAsync(this, "Add a program", "Enter a name for the program",
            programName);
        if (inputName == null) return;

        if (!string.IsNullOrEmpty(inputName)) programName = inputName;

        var settings = SettingsManager.Instance.Settings;
        if (settings.Programs.Any(p => p.Path == programPath)) return;

        Logger.Instance.Debug($"Adding new program '{programName}' with path '{programPath}'");

        settings.Programs.Add(new SettingsProgram
        {
            Path = programPath,
            Name = programName
        });
        SettingsManager.Instance.Save();
        UpdateList();
    }

    private async Task AddExtensionAsync()
    {
        var settings = SettingsManager.Instance.Settings;
        if (settings.Programs.Count == 0)
        {
            await MessageDialog.ShowAsync(this, "At least one program is required to add an extension");
            return;
        }

        var result = await ComboTextInputDialog.ShowAsync(this, "Add a file extension",
            "Enter a file extension (e.g. .sii)", "Select a default program for the extension",
            settings.Programs.Select(p => p.Name).ToList(), null, GetSelectedProgram()?.Name);
        if (result == null
            || string.IsNullOrWhiteSpace(result.Combo)
            || string.IsNullOrWhiteSpace(result.Text)) return;

        var extension = result.Text.Trim();
        if (!extension.StartsWith('.')) extension = $".{extension}";
        if (settings.Extensions.Any(x => x.Name == extension)) return;

        Logger.Instance.Debug($"Adding new extension '{extension}' with default program '{result.Combo}'");

        settings.Extensions.Add(new Extension {Name = extension, DefaultProgram = result.Combo});

        SettingsManager.Instance.Save();
        UpdateList();
    }

    #endregion

    #region Renaming and deleting

    private async Task RenameProgramAsync(SettingsProgram program)
    {
        var newProgramName = await TextInputDialog.ShowAsync(this, $"Renaming '{program.Name}'",
            "Enter a new program name", program.Name);

        if (string.IsNullOrWhiteSpace(newProgramName) || program.Name == newProgramName) return;

        Logger.Instance.Debug($"Renaming program from '{program.Name}' to '{newProgramName}'");

        var affectedExtensions = SettingsManager.Instance.Settings.Extensions
            .Where(x => x.DefaultProgram == program.Name).ToList();
        foreach (var ex in affectedExtensions)
        {
            ex.DefaultProgram = newProgramName;
            Logger.Instance.Debug(
                $"Renamed default program for '{ex.Name}' from '{program.Name}' to '{newProgramName}'");
        }

        program.Name = newProgramName;
        _hasChangesPending = true;
    }

    private async Task RenameExtensionAsync(Extension extension)
    {
        var newExtensionName = await TextInputDialog.ShowAsync(this, $"Renaming '{extension.Name}'",
            "Enter a new extension name", extension.Name);

        if (string.IsNullOrWhiteSpace(newExtensionName) || extension.Name == newExtensionName) return;

        Logger.Instance.Debug($"Renaming extension from '{extension.Name}' to '{newExtensionName}'");

        extension.Name = newExtensionName;
        _hasChangesPending = true;
    }

    private async Task DeleteProgramAsync(SettingsProgram program)
    {
        var settings = SettingsManager.Instance.Settings;
        if (settings.Extensions.Any(ex => ex.DefaultProgram == program.Name))
        {
            await MessageDialog.ShowAsync(this, "Cannot remove a program that has extension(s) assigned to it.",
                "Could not delete program.");
            return;
        }

        var result = await MessageDialog.ShowAsync(this,
            $"Are you sure you want to delete the program '{program.Name}'?", "Delete program?",
            MessageButtons.YesNo);
        if (result != MessageResult.Yes) return;

        Logger.Instance.Debug($"Deleting program '{program.Name}'");

        settings.Programs.Remove(program);
        _hasChangesPending = true;
    }

    private async Task DeleteExtensionAsync(Extension extension)
    {
        var result = await MessageDialog.ShowAsync(this,
            $"Are you sure you want to delete the extension '{extension.Name}'?", "Delete extension?",
            MessageButtons.YesNo);
        if (result != MessageResult.Yes) return;

        Logger.Instance.Debug($"Deleting extension '{extension.Name}'");

        SettingsManager.Instance.Settings.Extensions.Remove(extension);
        _hasChangesPending = true;
    }

    private void SetDefaultProgram(IEnumerable<Extension> extensions, SettingsProgram program)
    {
        foreach (var extension in extensions)
        {
            Logger.Instance.Debug(
                $"Setting default program for extension '{extension.Name}' from '{extension.DefaultProgram}' to '{program.Name}'");
            extension.DefaultProgram = program.Name;
        }

        UpdateList();
        _hasChangesPending = true;
    }

    private async void SettingsTree_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _dragStartPoint = null;
        _dragExtension = null;

        if (e.InitialPressMouseButton != MouseButton.Right) return;

        var item = GetTreeItem(e);
        if (item == null) return;

        e.Handled = true;

        if (SettingsTree.SelectedItems.Count > 1 && SettingsTree.SelectedItems.Contains(item))
        {
            await MessageDialog.ShowAsync(this, "You can only modify one item at a time");
            return;
        }

        // Right clicking an item that is not selected targets that item
        if (!SettingsTree.SelectedItems.Contains(item)) SettingsTree.SelectedItem = item;

        ShowItemContextMenu(item);
    }

    private void ShowItemContextMenu(TreeViewItem item)
    {
        var contextMenu = new ContextMenu();

        var renameMenuItem = new MenuItem {Header = "Rename"};
        renameMenuItem.Click += async (_, _) =>
        {
            if (item.Tag is SettingsProgram program)
                await RenameProgramAsync(program);
            else if (item.Tag is Extension extension) await RenameExtensionAsync(extension);

            UpdateList();
        };
        contextMenu.Items.Add(renameMenuItem);

        // Same as dragging the extension to a program, for when dragging is not convenient
        if (item.Tag is Extension selectedExtension && SettingsManager.Instance.Settings.Programs.Count > 1)
        {
            var assignMenuItem = new MenuItem {Header = "Set Default Program"};
            foreach (var program in SettingsManager.Instance.Settings.Programs.OrderBy(p => p.Name))
            {
                var targetProgram = program;
                var programMenuItem = new MenuItem
                {
                    Header = new TextBlock {Text = targetProgram.Name},
                    IsEnabled = targetProgram.Name != selectedExtension.DefaultProgram
                };
                programMenuItem.Click += (_, _) => SetDefaultProgram(new[] {selectedExtension}, targetProgram);
                assignMenuItem.Items.Add(programMenuItem);
            }

            contextMenu.Items.Add(assignMenuItem);
        }

        var deleteMenuItem = new MenuItem {Header = "Delete"};
        deleteMenuItem.Click += async (_, _) =>
        {
            if (item.Tag is SettingsProgram program)
                await DeleteProgramAsync(program);
            else if (item.Tag is Extension extension) await DeleteExtensionAsync(extension);

            UpdateList();
        };
        contextMenu.Items.Add(deleteMenuItem);

        contextMenu.Open(SettingsTree);
    }

    #endregion

    #region Drag and drop

    private void SettingsTree_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _dragStartPoint = null;
        _dragExtension = null;

        if (!e.GetCurrentPoint(SettingsTree).Properties.IsLeftButtonPressed) return;
        if (GetTreeItem(e)?.Tag is not Extension extension) return;

        _dragExtension = extension;
        _dragStartPoint = e.GetPosition(SettingsTree);
    }

    private async void SettingsTree_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragStartPoint is not { } start || _dragExtension == null) return;

        if (!e.GetCurrentPoint(SettingsTree).Properties.IsLeftButtonPressed)
        {
            _dragStartPoint = null;
            _dragExtension = null;
            return;
        }

        var current = e.GetPosition(SettingsTree);
        if (new Vector(current.X - start.X, current.Y - start.Y).Length < DragThreshold) return;

        var pressedExtension = _dragExtension;
        _dragStartPoint = null;
        _dragExtension = null;

        // Drag all the selected extensions when the pressed one is part of the selection
        var selectedExtensions = SettingsTree.SelectedItems.OfType<TreeViewItem>()
            .Select(i => i.Tag).OfType<Extension>().ToList();
        if (!selectedExtensions.Contains(pressedExtension)) selectedExtensions = new List<Extension> {pressedExtension};

        var dataTransfer = new DataTransfer();
        dataTransfer.Add(DataTransferItem.Create(ExtensionsFormat,
            string.Join('\n', selectedExtensions.Select(x => x.Name))));

        try
        {
            await DragDrop.DoDragDropAsync(e, dataTransfer, DragDropEffects.Move);
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"Could not start the drag and drop: {ex}");
        }
        finally
        {
            InfoLabel.Text = "";
        }
    }

    /// <summary>
    ///     Checks if the dragged extensions can be dropped at the item the pointer is over
    /// </summary>
    /// <returns>The program they would be assigned to, null when not allowed (<paramref name="message" /> says why)</returns>
    private SettingsProgram? GetDropTarget(DragEventArgs e, out List<string> extensionNames, out string message)
    {
        extensionNames = new List<string>();
        message = "";

        var draggedNames = e.DataTransfer.TryGetValue(ExtensionsFormat);
        if (string.IsNullOrEmpty(draggedNames))
        {
            message = "You can only assign extensions to programs";
            return null;
        }

        extensionNames = draggedNames.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

        var item = GetTreeItem(e);
        if (item?.Tag is not SettingsProgram program)
        {
            message = item?.Tag is Extension
                ? "Cannot assign an extension to another extension"
                : "Drop the extension on a program to assign it";
            return null;
        }

        message = $"Sets '{program.Name}' as the default program for '{string.Join(", ", extensionNames)}'";
        return program;
    }

    private void SettingsTree_DragOver(object? sender, DragEventArgs e)
    {
        var program = GetDropTarget(e, out _, out var message);
        InfoLabel.Text = message;
        InfoLabel.Foreground = (Avalonia.Media.IBrush?) (program != null
            ? this.FindResource("FoamBrush")
            : this.FindResource("LoveBrush"));
        e.DragEffects = program != null ? DragDropEffects.Move : DragDropEffects.None;
        e.Handled = true;
    }

    private void SettingsTree_Drop(object? sender, DragEventArgs e)
    {
        e.Handled = true;
        InfoLabel.Text = "";

        var program = GetDropTarget(e, out var extensionNames, out _);
        if (program == null) return;

        var extensions = SettingsManager.Instance.Settings.Extensions
            .Where(x => extensionNames.Contains(x.Name)).ToList();
        SetDefaultProgram(extensions, program);
    }

    #endregion

    #region Closing

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        if (_closeConfirmed || e.Cancel || !_hasChangesPending) return;

        // Never hold up the shutdown of the OS (or the application) with questions
        if (e.CloseReason is WindowCloseReason.OSShutdown or WindowCloseReason.ApplicationShutdown) return;

        // The question is asynchronous, so the close is cancelled and done again when it is answered
        e.Cancel = true;
        _ = ConfirmCloseAsync();
    }

    private async Task ConfirmCloseAsync()
    {
        var result = await MessageDialog.ShowAsync(this, "There are unsaved changes, do you want to save them?",
            "Save changes?", MessageButtons.YesNoCancel);

        switch (result)
        {
            case MessageResult.Yes:
                SettingsManager.Instance.Save();
                break;
            case MessageResult.No:
                SettingsManager.Instance.Load();
                break;
            default:
                // Cancelled (or the question was closed without answering), keep the window open
                return;
        }

        _closeConfirmed = true;
        Close();
    }

    #endregion
}
