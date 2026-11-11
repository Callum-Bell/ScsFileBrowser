using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using ScsFileBrowser.AvaloniaUI.Controls;
using ScsFileBrowser.AvaloniaUI.Models;
using ScsFileBrowser.AvaloniaUI.Services;
using ScsFileBrowser.AvaloniaUI.Views.Dialogs;
using ScsFileBrowser.FileSystem;
using ScsFileBrowser.Logging;
using ScsFileBrowser.Settings;
using ScsFileBrowser.Utils;
using SettingsProgram = ScsFileBrowser.Settings.Program;

namespace ScsFileBrowser.AvaloniaUI.Views;

public partial class MainWindow : Window
{
    /// <summary>
    ///     Distance (in pixels) the pointer has to move with the button down before a file drag starts
    /// </summary>
    private const double DragThreshold = 8;

    private bool _closeConfirmed;
    private string _currentPath = string.Empty;
    private Point? _dragStartPoint;
    private bool _isExtracting;
    private bool _isLoading;

    public MainWindow()
    {
        InitializeComponent();

        CurrentPathBar.PathChanged += CurrentPathBar_PathChanged;

        OpenFileMenuItem.Click += async (_, _) => await OpenFileFromDialogAsync();
        OpenFolderMenuItem.Click += async (_, _) => await OpenFolderFromDialogAsync();
        SettingsMenuItem.Click += async (_, _) => await new SettingsWindow().ShowDialog(this);

        FolderGrid.DoubleTapped += (_, e) =>
        {
            if (IsOnRow(e)) ActivateSelectedFolder();
        };
        FolderGrid.AddHandler(KeyDownEvent, FolderGrid_KeyDown, RoutingStrategies.Tunnel);
        FolderGrid.AddHandler(PointerReleasedEvent, FolderGrid_PointerReleased, RoutingStrategies.Bubble, true);

        FileGrid.DoubleTapped += (_, e) =>
        {
            if (IsOnRow(e)) ActivateSelectedFile();
        };
        FileGrid.AddHandler(KeyDownEvent, FileGrid_KeyDown, RoutingStrategies.Tunnel);
        FileGrid.AddHandler(PointerReleasedEvent, FileGrid_PointerReleased, RoutingStrategies.Bubble, true);
        FileGrid.AddHandler(PointerPressedEvent, FileGrid_PointerPressed, RoutingStrategies.Tunnel);
        FileGrid.AddHandler(PointerMovedEvent, FileGrid_PointerMoved, RoutingStrategies.Bubble, true);

        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, Window_DragOver);
        AddHandler(DragDrop.DropEvent, Window_Drop);

        RebuildRecentMenu();
        UpdateCurrentPathUi();
    }

    /// <param name="args">Command line arguments, the first one can be an archive file or a folder to open</param>
    public MainWindow(string[]? args) : this()
    {
        if (args is not {Length: > 0}) return;

        var possibleFilePath = args[0];
        if (File.Exists(possibleFilePath) &&
            Consts.AllowedDropExtensions.Contains(Path.GetExtension(possibleFilePath)))
            OpenArchiveFile(possibleFilePath);
        else if (Directory.Exists(possibleFilePath))
            OpenArchiveFolder(possibleFilePath);
    }

    #region Helpers

    private void UpdateCurrentPathUi()
    {
        CurrentPathBar.SetPath(_currentPath);
    }

    private void SetStatusMessage(string message)
    {
        StatusLabel.Text = $"[{DateTime.Now:HH:mm:ss}] {message}";
    }

    private Task<MessageResult> ShowMessageAsync(string message, string title = "ScsFileBrowser",
        MessageButtons buttons = MessageButtons.Ok)
    {
        return MessageDialog.ShowAsync(this, message, title, buttons);
    }

    private void ShowBusyIndicator(string labelText)
    {
        BusyLabel.Text = labelText;
        BusyPanel.IsVisible = true;
    }

    private void RemoveBusyIndicator()
    {
        BusyPanel.IsVisible = false;
    }

    /// <summary>
    ///     Shows <paramref name="text" /> in a tooltip on <paramref name="target" /> for a short time
    /// </summary>
    private static async void ShowHint(Control target, string text)
    {
        ToolTip.SetTip(target, text);
        ToolTip.SetIsOpen(target, true);
        await Task.Delay(2000);
        ToolTip.SetIsOpen(target, false);
        ToolTip.SetTip(target, null);
    }

    private static bool IsOnRow(RoutedEventArgs e)
    {
        return e.Source is Visual source && source.FindAncestorOfType<DataGridRow>(true) != null;
    }

    private static bool IsOnColumnHeader(RoutedEventArgs e)
    {
        return e.Source is Visual source && source.FindAncestorOfType<DataGridColumnHeader>(true) != null;
    }

    /// <summary>
    ///     Prefixes <paramref name="localFolderName" /> with <see cref="_currentPath" />
    /// </summary>
    private string GetFullPathForLocalItem(string localFolderName)
    {
        var dirPath = PathUtils.CombinePath(_currentPath, localFolderName);

        if (dirPath.EndsWith('/')) dirPath = dirPath[..^1];

        return dirPath;
    }

    private bool CheckIfLoading()
    {
        if (_isLoading) _ = ShowMessageAsync("Archive files are still being loaded, please wait.");
        return _isLoading;
    }

    private void GoToParentFolder()
    {
        var newPath = "";
        if (_currentPath.Contains('/'))
        {
            var lastSlash = _currentPath.LastIndexOf('/');
            newPath = _currentPath[..lastSlash];
        }

        ChangePath(newPath);
    }

    #endregion

    #region Navigation

    private bool ChangePath(string newPath)
    {
        var dirEntries = UberFileSystem.Instance.GetDirectoryEntries(newPath);
        if (dirEntries.Count == 0)
        {
            SetStatusMessage($"Could not find directory '{newPath}'");
            Logger.Instance.Error($"Could not find directory '{newPath}'");
            return false;
        }

        _currentPath = newPath;

        var subFolderList = new List<FolderRow>();
        foreach (var subDirectoryName in dirEntries.SelectMany(de => de.GetSubDirectoryNames()).Distinct())
        {
            var subFolderPath = GetFullPathForLocalItem(subDirectoryName);
            var subFolder = UberFileSystem.Instance.GetDirectoryEntries(subFolderPath);
            if (subFolder.Count == 0)
            {
                Logger.Instance.Error($"Could not find folder '{subDirectoryName}' in '{_currentPath}'");
                continue;
            }

            subFolderList.Add(new FolderRow
            {
                Name = subDirectoryName,
                SubEntryCount = subFolder.SelectMany(sf => sf.GetFiles().Concat(sf.GetSubDirectoryNames())).Distinct()
                    .Count()
            });
        }

        FolderGrid.ItemsSource = subFolderList.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase).ToList();

        var subFileList = new List<FileRow>();
        foreach (var subFileName in dirEntries.SelectMany(de => de.GetFiles()).Distinct())
        {
            var subFilePath = GetFullPathForLocalItem(subFileName);
            var subFile = UberFileSystem.Instance.GetFileEntries(subFilePath);
            if (subFile.Count == 0)
            {
                Logger.Instance.Error($"Could not find file '{subFileName}' in '{_currentPath}'");
                continue;
            }

            var lastFile = subFile.Last();
            var source = lastFile.Entry.GetArchiveFile().GetName();
            if (subFile.Count > 1) source += $" (+{subFile.Count - 1})";

            subFileList.Add(new FileRow
            {
                Name = subFileName,
                Source = source,
                Size = lastFile.Entry.Size,
                SourcesTooltip = subFile.Count > 1
                    ? string.Join('\n', subFile.Select(f => f.Entry.GetArchiveFile().GetName()))
                    : null
            });
        }

        FileGrid.ItemsSource = subFileList.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase).ToList();

        UpdateCurrentPathUi();

        return true;
    }

    private void Reset()
    {
        UberFileSystem.Instance.Reset();
        FolderGrid.ItemsSource = null;
        FileGrid.ItemsSource = null;
        _currentPath = "";
        UpdateCurrentPathUi();
    }

    private void CurrentPathBar_PathChanged(object? sender, PathChangedEventArgs e)
    {
        if (CheckIfLoading()) return;
        if (!ChangePath(e.NewPath))
        {
            Logger.Instance.Debug($"The path '{e.NewPath}' was not found in the current archive(s)");
            ShowHint(CurrentPathBar, "The selected path was not found in the archive(s).");
        }

        UpdateCurrentPathUi();
    }

    private void ActivateSelectedFolder()
    {
        if (CheckIfLoading()) return;
        if (FolderGrid.SelectedItem is not FolderRow selectedFolder) return;
        ChangePath(GetFullPathForLocalItem(selectedFolder.Name));
    }

    private void ActivateSelectedFile()
    {
        if (CheckIfLoading()) return;
        if (_isExtracting)
        {
            _ = ShowMessageAsync("Cannot open file(s) when an extraction is running.");
            return;
        }

        if (FileGrid.SelectedItem is not FileRow selectedItem) return;

        OpenFile(GetFullPathForLocalItem(selectedItem.Name));
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        // Go up a directory when backspace is pressed while not typing in a text box
        if (e.Handled || e.Key != Key.Back || CurrentPathBar.IsEditing || e.Source is TextBox) return;

        e.Handled = true;
        if (CheckIfLoading()) return;
        GoToParentFolder();
    }

    private void FolderGrid_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        e.Handled = true;
        ActivateSelectedFolder();
    }

    private void FileGrid_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        e.Handled = true;
        ActivateSelectedFile();
    }

    #endregion

    #region Opening archives

    private void AddToRecentHistory(string path)
    {
        var settings = SettingsManager.Instance.Settings;
        settings.RecentPaths.Remove(path);
        settings.RecentPaths.Insert(0, path);
        settings.RecentPaths = settings.RecentPaths.Take(10).ToList();

        SettingsManager.Instance.Save();
        RebuildRecentMenu();
    }

    private void RebuildRecentMenu()
    {
        OpenRecentMenuItem.Items.Clear();

        var recentPaths = SettingsManager.Instance.Settings.RecentPaths;
        OpenRecentMenuItem.IsEnabled = recentPaths.Count > 0;

        foreach (var recentPath in recentPaths)
        {
            var path = recentPath;
            // A TextBlock as header, so '_' in paths is not treated as an access key marker
            var menuItem = new MenuItem {Header = new TextBlock {Text = path}};
            menuItem.Click += (_, _) => OpenRecentPath(path);
            OpenRecentMenuItem.Items.Add(menuItem);
        }
    }

    private void OpenRecentPath(string path)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            SetStatusMessage($"Could not open the recent path '{path}'");
            Logger.Instance.Error($"Could not open the recent path '{path}'");
            return;
        }

        Logger.Instance.Debug($"Opening recent path '{path}'");
        if (Directory.Exists(path))
            OpenArchiveFolder(path);
        else
            OpenArchiveFile(path);
    }

    private async Task OpenFileFromDialogAsync()
    {
        var path = await PickerService.PickFileAsync(this, "Open an SCS archive",
            SettingsManager.Instance.Settings.LastSelectedFile, PickerService.ArchiveFileTypes);
        if (path == null) return;

        SettingsManager.Instance.Settings.LastSelectedFile = path;
        SettingsManager.Instance.Save();

        OpenArchiveFile(path);
    }

    private async Task OpenFolderFromDialogAsync()
    {
        var path = await PickerService.PickFolderAsync(this, "Open a folder containing SCS archives",
            SettingsManager.Instance.Settings.LastSelectedFolder);
        if (path == null) return;

        SettingsManager.Instance.Settings.LastSelectedFolder = path;
        SettingsManager.Instance.Save();

        OpenArchiveFolder(path);
    }

    internal void OpenArchiveFile(string path)
    {
        if (CheckIfLoading()) return;

        if (_isExtracting)
        {
            _ = ShowMessageAsync("Cannot open an archive file while an extraction is running");
            return;
        }

        Reset();

        UberFileSystem.Instance.AddSourceFile(path);

        Logger.Instance.Info($"Opened File: {path}");
        SetStatusMessage($"Opened File: {path}");
        Title = $"ScsFileBrowser - {path}";
        ChangePath("");
        AddToRecentHistory(path);
    }

    internal async void OpenArchiveFolder(string path)
    {
        if (CheckIfLoading()) return;

        if (_isExtracting)
        {
            _ = ShowMessageAsync("Cannot open a folder while an extraction is running");
            return;
        }

        Reset();
        ShowBusyIndicator("Loading archive files");
        _isLoading = true;

        try
        {
            await Task.Run(() => UberFileSystem.Instance.AddSourceDirectory(path));
        }
        catch (Exception e)
        {
            Logger.Instance.Error($"Could not load directory: {e}");
            SetStatusMessage($"Error opening '{path}'");
            Title = "ScsFileBrowser";
            return;
        }
        finally
        {
            _isLoading = false;
            RemoveBusyIndicator();
        }

        Logger.Instance.Info($"Opened Folder: {path}");
        SetStatusMessage($"Opened Folder: {path}");
        Title = $"ScsFileBrowser - {path}";
        ChangePath("");
        AddToRecentHistory(path);
    }

    #endregion

    #region Extracting and opening files

    private static string? ExtractFile(UberFile file, string extractionFolder, string originalPath,
        string? customFileName = null)
    {
        try
        {
            return CommonUtils.ExtractFile(file, extractionFolder, originalPath, customFileName);
        }
        catch (Exception e)
        {
            Logger.Instance.Error($"Could not extract the file '{originalPath}': {e.Message}");
        }

        return null;
    }

    private static void ExtractFolder(string path, string extractionFolder)
    {
        var dirEntries = UberFileSystem.Instance.GetDirectoryEntries(path);

        if (dirEntries.Count == 0)
        {
            Logger.Instance.Error($"Could not find folder '{path}'");
            return;
        }

        foreach (var subFolderName in dirEntries.SelectMany(de => de.GetSubDirectoryNames()).Distinct())
            ExtractFolder(PathUtils.CombinePath(path, subFolderName), extractionFolder);

        foreach (var fileName in dirEntries.SelectMany(de => de.GetFiles()).Distinct())
        {
            var filePath = PathUtils.CombinePath(path, fileName);
            var file = UberFileSystem.Instance.GetFileEntries(filePath).LastOrDefault();

            if (file == null)
            {
                Logger.Instance.Error($"Could not find file '{filePath}'");
                continue;
            }

            ExtractFile(file, extractionFolder, filePath, filePath);
        }
    }

    private async Task ExtractFoldersPromptAsync(IReadOnlyList<string> paths)
    {
        if (_isExtracting)
        {
            await ShowMessageAsync("Cannot have multiple extractions running at once.");
            return;
        }

        var extractionFolder = await PickerService.PickFolderAsync(this, "Select a folder to extract to",
            SettingsManager.Instance.Settings.LastExportPath);
        if (extractionFolder == null) return;

        // Another extraction could have been started while the picker was open
        if (_isExtracting)
        {
            await ShowMessageAsync("Cannot have multiple extractions running at once.");
            return;
        }

        SettingsManager.Instance.Settings.LastExportPath = extractionFolder;
        SettingsManager.Instance.Save();

        var pathsText = string.Join(", ", paths);
        SetStatusMessage($"Extracting folder(s) {pathsText}");
        Logger.Instance.Debug($"Extracting folder(s) {pathsText}");

        _isExtracting = true;
        ShowBusyIndicator("Extracting folder");
        try
        {
            await Task.Run(() =>
            {
                foreach (var path in paths)
                {
                    ExtractFolder(path, extractionFolder);
                    Logger.Instance.Info($"Finished extracting folder '{path}'");
                }
            });

            Logger.Instance.Info($"Finished extracting folder(s) {pathsText}");
            SetStatusMessage($"Finished extracting folder(s) {pathsText}");
        }
        catch (Exception e)
        {
            Logger.Instance.Error($"Extraction task failed to complete: {e}");
            SetStatusMessage("Extraction failed, see the log for details");
        }
        finally
        {
            _isExtracting = false;
            RemoveBusyIndicator();
        }
    }

    private async Task ExtractFilesPromptAsync(IReadOnlyList<string> paths)
    {
        if (_isExtracting)
        {
            await ShowMessageAsync("Cannot have multiple extractions running at once.");
            return;
        }

        var extractionFolder = await PickerService.PickFolderAsync(this, "Select a folder to extract to",
            SettingsManager.Instance.Settings.LastExportPath);
        if (extractionFolder == null) return;

        if (_isExtracting)
        {
            await ShowMessageAsync("Cannot have multiple extractions running at once.");
            return;
        }

        SetStatusMessage("Extracting files...");
        Logger.Instance.Debug($"Extracting files ({string.Join(", ", paths)})");

        SettingsManager.Instance.Settings.LastExportPath = extractionFolder;
        SettingsManager.Instance.Save();

        _isExtracting = true;
        ShowBusyIndicator("Extracting files");
        try
        {
            var extractedCount = await Task.Run(() =>
            {
                var count = 0;
                foreach (var filePath in paths)
                {
                    var file = UberFileSystem.Instance.GetFileEntries(filePath).LastOrDefault();
                    if (file == null)
                    {
                        Logger.Instance.Error($"Could not find file '{filePath}'");
                        continue;
                    }

                    if (ExtractFile(file, extractionFolder, filePath, filePath) != null) count++;
                }

                return count;
            });

            SetStatusMessage($"Finished extracting {extractedCount} file(s).");
            Logger.Instance.Info($"Finished extracting {extractedCount} file(s).");
        }
        catch (Exception e)
        {
            Logger.Instance.Error($"Extraction task failed to complete: {e}");
            SetStatusMessage("Extraction failed, see the log for details");
        }
        finally
        {
            _isExtracting = false;
            RemoveBusyIndicator();
        }
    }

    private async Task CopyFolderContentAsTextAsync(IReadOnlyList<string> paths)
    {
        var sb = new StringBuilder();
        var versionFile = UberFileSystem.Instance.GetFileEntries("version.txt").LastOrDefault();
        var isUsa = UberFileSystem.Instance.GetFileEntries("map/usa.mbd").LastOrDefault() != null;
        var isEu = UberFileSystem.Instance.GetFileEntries("map/europe.mbd").LastOrDefault() != null;
        string? gameInfo = null;
        if ((isUsa || isEu) && versionFile != null)
        {
            gameInfo = isUsa ? "American Truck Simulator" : "Euro Truck Simulator 2";
            gameInfo += $" (v{Encoding.UTF8.GetString(versionFile.Entry.Read()).TrimEnd('\n')})";
        }

        for (var i = 0; i < paths.Count; i++)
        {
            var path = paths[i];
            var dirEntries = UberFileSystem.Instance.GetDirectoryEntries(path);

            if (dirEntries.Count == 0) continue;

            if (i > 0) sb.AppendLine();

            sb.AppendLine($"=== Contents of '{path}'{(gameInfo != null ? $" for {gameInfo}" : "")} ===");

            var subDirNames = dirEntries.SelectMany(de => de.GetSubDirectoryNames()).Distinct().OrderBy(x => x);

            foreach (var subDirectoryName in subDirNames)
                sb.AppendLine($"/{PathUtils.CombinePath(path, subDirectoryName)}");

            var subFiles = dirEntries.SelectMany(de => de.GetFiles()).Distinct().OrderBy(x => x);
            foreach (var fileName in subFiles) sb.AppendLine($"/{PathUtils.CombinePath(path, fileName)}");
        }

        await ClipboardService.SetTextAsync(this, sb.ToString());

        SetStatusMessage("Copied folder contents to clipboard");
    }

    private void OpenFile(string path, SettingsProgram program)
    {
        var file = UberFileSystem.Instance.GetFileEntries(path);

        if (file.Count == 0)
        {
            SetStatusMessage($"Could not find file '{path}'");
            Logger.Instance.Error($"Could not find file '{path}'");
            return;
        }

        if (!File.Exists(program.Path))
        {
            SetStatusMessage($"The executable ('{program.Path}') for program '{program.Name}' does not exist.");
            Logger.Instance.Error($"The executable ('{program.Path}') for program '{program.Name}' does not exist.");
            return;
        }

        var fileOutPath = ExtractFile(file.First(), Consts.TempFileDirPath, path);
        if (fileOutPath == null) return;

        try
        {
            ProgramLauncher.OpenWithProgram(program.Path, fileOutPath);
        }
        catch (Exception e)
        {
            SetStatusMessage($"Could not start '{program.Name}', see the log for details");
            Logger.Instance.Error($"Could not start '{program.Name}' ('{program.Path}'): {e.Message}");
            return;
        }

        SetStatusMessage($"File '{path}' opened with '{program.Name}'");
        Logger.Instance.Info($"File '{path}' opened with '{program.Name}' ('{program.Path}')");
    }

    /// <summary>
    ///     Open the file for the given path with the default application associated with the file extension set in settings
    /// </summary>
    /// <param name="path">Path of the file (in archive) to open</param>
    private void OpenFile(string path)
    {
        var extStr = Path.GetExtension(path);

        var ext = SettingsManager.Instance.Settings.Extensions.Find(x => x.Name == extStr);

        if (ext == null || string.IsNullOrWhiteSpace(ext.DefaultProgram))
        {
            SetStatusMessage(
                $"Could not find default program for extension '{extStr}', head over to the settings if you want to add it");
            Logger.Instance.Error($"Could not find default program for extension '{extStr}'");
            return;
        }

        var program = SettingsManager.Instance.Settings.Programs.Find(p => p.Name == ext.DefaultProgram);

        if (program == null)
        {
            SetStatusMessage($"Could not find program '{ext.DefaultProgram}'");
            Logger.Instance.Error($"Could not find program '{ext.DefaultProgram}'");
            return;
        }

        OpenFile(path, program);
    }

    private void OpenFiles(IEnumerable<string> paths)
    {
        foreach (var path in paths) OpenFile(path);
    }

    private void OpenFiles(IEnumerable<string> paths, SettingsProgram program)
    {
        foreach (var path in paths) OpenFile(path, program);
    }

    #endregion

    #region Context menus

    private void FolderGrid_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton != MouseButton.Right || IsOnColumnHeader(e)) return;
        if (CheckIfLoading()) return;

        var selectedPaths = new List<string>();
        // Right clicking the empty space below the rows targets the current folder
        if (!IsOnRow(e) || FolderGrid.SelectedItems.Count == 0)
        {
            FolderGrid.SelectedItems.Clear();
            selectedPaths.Add(_currentPath);
        }
        else
        {
            selectedPaths.AddRange(FolderGrid.SelectedItems.OfType<FolderRow>()
                .Select(folder => GetFullPathForLocalItem(folder.Name)));
            if (selectedPaths.Count == 0) return;
        }

        e.Handled = true;
        ShowFolderContextMenu(selectedPaths, e.KeyModifiers.HasFlag(KeyModifiers.Shift));
    }

    private void ShowFolderContextMenu(List<string> selectedPaths, bool shiftHeld)
    {
        var hasMultiple = selectedPaths.Count > 1;
        var contextMenu = new ContextMenu();

        var copyPathMenuItem = new MenuItem {Header = hasMultiple ? "Copy Paths" : "Copy Path"};
        copyPathMenuItem.Click += async (_, _) =>
            await ClipboardService.SetTextAsync(this, string.Join("\n", selectedPaths));
        contextMenu.Items.Add(copyPathMenuItem);

        if (shiftHeld)
        {
            if (!_isExtracting)
            {
                var subItemsAsTextMenuItem = new MenuItem {Header = "Copy sub folder/file names"};
                ToolTip.SetTip(subItemsAsTextMenuItem,
                    $"Copies the sub folder and file names for the {(hasMultiple ? "directories" : "directory")} to the clipboard");
                subItemsAsTextMenuItem.Click += async (_, _) => await CopyFolderContentAsTextAsync(selectedPaths);
                contextMenu.Items.Add(subItemsAsTextMenuItem);
            }

            var copyCityHashMenuItem = new MenuItem {Header = "Copy CityHash Value"};
            ToolTip.SetTip(copyCityHashMenuItem,
                $"Copies the cityhash value for the {(hasMultiple ? "directories" : "directory")} to the clipboard");
            copyCityHashMenuItem.Click += async (_, _) =>
                await ClipboardService.SetTextAsync(this,
                    string.Join("\n", selectedPaths.Select(p => $"{CityHash.CityHash64(p):X}")));
            contextMenu.Items.Add(copyCityHashMenuItem);
        }

        if (!_isExtracting)
        {
            var extractFolderMenuItem = new MenuItem {Header = hasMultiple ? "Extract Folders" : "Extract Folder"};
            ToolTip.SetTip(extractFolderMenuItem,
                $"Extracts the {(hasMultiple ? "directories" : "directory")} with the current path '{_currentPath}' prefixed");
            extractFolderMenuItem.Click += async (_, _) => await ExtractFoldersPromptAsync(selectedPaths);
            contextMenu.Items.Add(extractFolderMenuItem);
        }

        contextMenu.Open(FolderGrid);
    }

    private void FileGrid_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _dragStartPoint = null;

        if (e.InitialPressMouseButton != MouseButton.Right || !IsOnRow(e)) return;
        if (CheckIfLoading()) return;

        var selectedPaths = FileGrid.SelectedItems.OfType<FileRow>()
            .Select(file => GetFullPathForLocalItem(file.Name)).ToList();
        if (selectedPaths.Count == 0) return;

        e.Handled = true;
        ShowFileContextMenu(selectedPaths, e.KeyModifiers.HasFlag(KeyModifiers.Shift));
    }

    private void ShowFileContextMenu(List<string> selectedPaths, bool shiftHeld)
    {
        var hasMultiple = selectedPaths.Count > 1;
        var contextMenu = new ContextMenu();

        if (!_isExtracting)
        {
            var openFileMenuItem = new MenuItem {Header = "Open"};
            openFileMenuItem.Click += (_, _) => OpenFiles(selectedPaths);
            contextMenu.Items.Add(openFileMenuItem);

            var programs = SettingsManager.Instance.Settings.Programs;
            if (programs.Count > 0)
            {
                var openFileWithMenuItem = new MenuItem {Header = "Open With"};
                foreach (var program in programs)
                {
                    var programMenuItem = new MenuItem {Header = new TextBlock {Text = program.Name}};
                    var selectedProgram = program;
                    programMenuItem.Click += (_, _) => OpenFiles(selectedPaths, selectedProgram);
                    openFileWithMenuItem.Items.Add(programMenuItem);
                }

                contextMenu.Items.Add(openFileWithMenuItem);
            }
        }

        var copyPathMenuItem = new MenuItem {Header = hasMultiple ? "Copy Paths" : "Copy Path"};
        copyPathMenuItem.Click += async (_, _) =>
            await ClipboardService.SetTextAsync(this, string.Join("\n", selectedPaths.Select(p => $"/{p}")));
        contextMenu.Items.Add(copyPathMenuItem);

        if (shiftHeld)
        {
            var copyCityHashMenuItem = new MenuItem {Header = "Copy CityHash Value"};
            ToolTip.SetTip(copyCityHashMenuItem,
                $"Copies the cityhash value for the {(hasMultiple ? "files" : "file")} to the clipboard");
            copyCityHashMenuItem.Click += async (_, _) =>
                await ClipboardService.SetTextAsync(this,
                    string.Join("\n", selectedPaths.Select(p => $"{CityHash.CityHash64(p):X}")));
            contextMenu.Items.Add(copyCityHashMenuItem);
        }

        if (!_isExtracting)
        {
            var extractFileMenuItem = new MenuItem {Header = hasMultiple ? "Extract Files" : "Extract File"};
            ToolTip.SetTip(extractFileMenuItem,
                $"Extracts the {(hasMultiple ? "files" : "file")} with the current path '{_currentPath}' prefixed");
            extractFileMenuItem.Click += async (_, _) => await ExtractFilesPromptAsync(selectedPaths);
            contextMenu.Items.Add(extractFileMenuItem);
        }

        contextMenu.Open(FileGrid);
    }

    #endregion

    #region Drag and drop

    private void FileGrid_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _dragStartPoint = null;
        if (!e.GetCurrentPoint(FileGrid).Properties.IsLeftButtonPressed || !IsOnRow(e)) return;

        _dragStartPoint = e.GetPosition(FileGrid);
    }

    private async void FileGrid_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragStartPoint is not { } start) return;

        if (!e.GetCurrentPoint(FileGrid).Properties.IsLeftButtonPressed)
        {
            _dragStartPoint = null;
            return;
        }

        var current = e.GetPosition(FileGrid);
        if (new Vector(current.X - start.X, current.Y - start.Y).Length < DragThreshold) return;

        _dragStartPoint = null;
        try
        {
            await StartFileDragAsync(e);
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"Could not start the drag and drop: {ex}");
        }
    }

    /// <summary>
    ///     Extracts the selected files to the temp directory and lets the user drag them to other applications
    /// </summary>
    private async Task StartFileDragAsync(PointerEventArgs triggerEvent)
    {
        if (CheckIfLoading()) return;
        if (_isExtracting)
        {
            await ShowMessageAsync("Cannot extract file(s) when an extraction is already running.");
            return;
        }

        var selectedItems = FileGrid.SelectedItems.OfType<FileRow>().ToList();
        if (selectedItems.Count == 0) return;

        var dataTransfer = new DataTransfer();
        foreach (var selectedItem in selectedItems)
        {
            var filePath = GetFullPathForLocalItem(selectedItem.Name);
            var file = UberFileSystem.Instance.GetFileEntries(filePath).LastOrDefault();

            if (file == null)
            {
                Logger.Instance.Error($"Could not extract file '{filePath}'");
                continue;
            }

            var extractedPath = ExtractFile(file, Consts.TempFileDirPath, filePath, filePath);
            if (extractedPath == null) continue;

            var storageFile = await StorageProvider.TryGetFileFromPathAsync(extractedPath);
            if (storageFile == null)
            {
                Logger.Instance.Error($"Could not find the extracted file '{extractedPath}'");
                continue;
            }

            dataTransfer.Add(DataTransferItem.CreateFile(storageFile));
        }

        if (dataTransfer.Items.Count == 0)
        {
            // DataTransfer only implements Dispose explicitly
            ((IDisposable) dataTransfer).Dispose();
            return;
        }

        Logger.Instance.Debug(
            $"Drag dropping files: {string.Join(", ", selectedItems.Select(f => GetFullPathForLocalItem(f.Name)))}");
        await DragDrop.DoDragDropAsync(triggerEvent, dataTransfer, DragDropEffects.Copy);
    }

    private void Window_DragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = !_isExtracting && !_isLoading && e.DataTransfer.Contains(DataFormat.File)
            ? DragDropEffects.Link
            : DragDropEffects.None;
        e.Handled = true;
    }

    private async void Window_Drop(object? sender, DragEventArgs e)
    {
        e.Handled = true;
        if (e.DataTransfer.TryGetFiles() is not {Length: > 0} items) return;

        var path = items[0].TryGetLocalPath();
        if (path == null) return;

        if (Directory.Exists(path))
        {
            OpenArchiveFolder(path);
            return;
        }

        if (Consts.AllowedDropExtensions.Contains(Path.GetExtension(path)))
        {
            OpenArchiveFile(path);
            return;
        }

        var err =
            $"File '{path}' did not have an allowed file extension ({string.Join(", ", Consts.AllowedDropExtensions)})";
        Logger.Instance.Error(err);
        await ShowMessageAsync(err, "Invalid file type");
    }

    #endregion

    #region Closing

    private static bool HasTempFiles()
    {
        return Directory.Exists(Consts.TempFileDirPath) &&
               Directory.EnumerateFileSystemEntries(Consts.TempFileDirPath).Any();
    }

    private static void CleanupTempFiles()
    {
        if (!Directory.Exists(Consts.TempFileDirPath)) return;

        foreach (var subFile in Directory.GetFiles(Consts.TempFileDirPath))
            try
            {
                File.Delete(subFile);
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"Could not delete temporary file '{subFile}': {e.Message}");
            }

        foreach (var subDir in Directory.GetDirectories(Consts.TempFileDirPath))
            try
            {
                Directory.Delete(subDir, true);
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"Could not delete temporary folder '{subDir}': {e.Message}");
            }
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        if (_closeConfirmed || e.Cancel) return;

        // Never hold up the shutdown of the OS (or the application) with questions
        if (e.CloseReason is WindowCloseReason.OSShutdown or WindowCloseReason.ApplicationShutdown) return;

        if (!_isExtracting && !HasTempFiles()) return;

        // The questions are asynchronous, so the close is cancelled and done again when they are answered
        e.Cancel = true;
        _ = ConfirmCloseAsync();
    }

    private async Task ConfirmCloseAsync()
    {
        try
        {
            if (_isExtracting)
            {
                var result = await ShowMessageAsync("An extraction is still running, are you sure you want to quit?",
                    "Stop extraction and close?", MessageButtons.YesNo);
                if (result != MessageResult.Yes) return;
            }

            if (HasTempFiles())
            {
                var result = await ShowMessageAsync("Do you want to remove all the temporarily extracted files?",
                    "Cleanup temporary files?", MessageButtons.YesNo);
                if (result == MessageResult.Yes) CleanupTempFiles();
            }
        }
        catch (Exception e)
        {
            Logger.Instance.Error($"Error while closing: {e}");
        }

        _closeConfirmed = true;
        Close();
    }

    #endregion
}
