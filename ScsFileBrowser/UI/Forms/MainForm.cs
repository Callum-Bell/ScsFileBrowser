using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using BrightIdeasSoftware;
using ScsFileBrowser.FileSystem;
using ScsFileBrowser.Logging;
using ScsFileBrowser.Model;
using ScsFileBrowser.Settings;
using ScsFileBrowser.UI.Components;
using ScsFileBrowser.Utils;

namespace ScsFileBrowser.UI.Forms;

public partial class MainForm : Form
{
    private string _currentPath = string.Empty;
    private bool _isExtracting;
    private bool _isLoading;

    public MainForm()
    {
        InitializeComponent();
        FileMenuBtn.Menu = FileContextMenuStrip;
        FileContextMenuStrip.Renderer = new ThemedToolStripMenuRenderer();
        var fileFilter = string.Join(";", Consts.AllowedDropExtensions.Select(e => $"*{e}"));
        BrowseFileDialog.Filter = $"SCS archives ({fileFilter})|{fileFilter}";

        CurrentPathFlowPanel.PathChanged += CurrentPathFlowPanel_PathChanged;
        SetTheme();
        InitColumns();
        UpdateCurrentPathUi();

        FileListView.CellToolTipShowing += FileListViewOnCellToolTipShowing;
    }

    private void FileListViewOnCellToolTipShowing(object? sender, ToolTipShowingEventArgs e)
    {
        if (e.ColumnIndex != 1) return;
        if (e.Model is not FileModel model) return;
        if (model.FileEntries.Count < 2) return;

        var sources = model.FileEntries.Select(f => f.Entry.GetArchiveFile().GetName());
        e.Text = string.Join('\n', sources);
    }

    private void InitColumns()
    {
        var folderNameColumn = new OLVColumn
        {
            Text = "Folder Name",
            AspectName = "Name",
            Width = 240,
            Sortable = true,
            Searchable = true
        };
        FolderListView.Columns.AddRange(new[]
        {
            folderNameColumn,
            new OLVColumn
            {
                Text = "Sub Items",
                AspectName = "SubEntryCount",
                Width = 80,
                Sortable = true
            },
            new OLVColumn
            {
                Text = "",
                IsEditable = false,
                Sortable = false,
                Hideable = false,
                FillsFreeSpace = true
            }
        });

        var fileNameColumn = new OLVColumn
        {
            Text = "File Name",
            AspectName = "Name",
            Width = 240,
            Sortable = true,
            Searchable = true
        };
        FileListView.Columns.AddRange(new[]
        {
            fileNameColumn,
            new OLVColumn
            {
                Text = "Source",
                AspectName = "Source",
                Width = 120,
                Sortable = true
            },
            new OLVColumn
            {
                Text = "Size",
                AspectName = "Size",
                Width = 60,
                Sortable = true,
                HeaderTextAlign = HorizontalAlignment.Left,
                TextAlign = HorizontalAlignment.Right,
                AspectToStringConverter = value => value is not uint size ? "N/A" : CommonUtils.FormatSize(size)
            },
            new OLVColumn
            {
                Text = "",
                IsEditable = false,
                Sortable = false,
                Hideable = false,
                FillsFreeSpace = true
            }
        });
        FolderListView.LastSortColumn = folderNameColumn;
        FolderListView.LastSortOrder = SortOrder.Ascending;
        FileListView.LastSortColumn = fileNameColumn;
        FileListView.LastSortOrder = SortOrder.Ascending;
    }

    private void UpdateCurrentPathUi()
    {
        CurrentPathFlowPanel.SetPath(_currentPath);
    }

    private void SetTheme()
    {
        TopPanel.BackColor = ThemedColors.Base;
        BackColor = ThemedColors.Surface;
        ForeColor = ThemedColors.Text;
        CurrentPathLbl.ForeColor = ThemedColors.Text;
        HelpToolTip.BackColor = ThemedColors.Base;

        OpenFileToolStripMenuItem.DropDown.BackColor = ThemedColors.Base;
        ((ToolStripDropDownMenu) OpenFileToolStripMenuItem.DropDown).ShowImageMargin = false;

        OpenFolderToolStripMenuItem.DropDown.BackColor = ThemedColors.Base;
        ((ToolStripDropDownMenu) OpenFolderToolStripMenuItem.DropDown).ShowImageMargin = false;

        OpenRecentToolStripMenuItem.DropDown.BackColor = ThemedColors.Base;
        ((ToolStripDropDownMenu) OpenRecentToolStripMenuItem.DropDown).ShowImageMargin = false;

        FileContextMenuStrip.BackColor = ThemedColors.Base;
        BusyLabel.ForeColor = ThemedColors.Gold;

        FolderListView.BackColor = ThemedColors.Overlay;
        FolderListView.ForeColor = ThemedColors.Text;
        FileListView.BackColor = ThemedColors.Overlay;
        FileListView.ForeColor = ThemedColors.Text;
        splitContainer1.BackColor = ThemedColors.HighlightHigh;

        FileListView.SelectedBackColor = ThemedColors.Pine;
        FileListView.UnfocusedSelectedBackColor = ThemedColors.HighlightLow;
        FileListView.UnfocusedSelectedForeColor = ThemedColors.Subtle;
        FileListView.HotItemStyle = new HotItemStyle {BackColor = ThemedColors.HighlightLow};

        FolderListView.SelectedBackColor = ThemedColors.Pine;
        FolderListView.UnfocusedSelectedBackColor = ThemedColors.HighlightLow;
        FolderListView.UnfocusedSelectedForeColor = ThemedColors.Subtle;
        FolderListView.HotItemStyle = new HotItemStyle {BackColor = ThemedColors.HighlightLow};

        var headerFormatStyle = new HeaderFormatStyle();
        headerFormatStyle.SetForeColor(ThemedColors.Text);
        headerFormatStyle.SetBackColor(ThemedColors.Base);
        headerFormatStyle.Hot.BackColor = ThemedColors.HighlightMed;
        FolderListView.HeaderFormatStyle = headerFormatStyle;
        FileListView.HeaderFormatStyle = headerFormatStyle;

        statusStrip1.BackColor = ThemedColors.Base;
        StatusLabel.ForeColor = ThemedColors.Text;
    }

    /// <summary>
    ///     Prefixes <paramref name="localFolderName" /> with <see cref="_currentPath" />
    /// </summary>
    /// <param name="localFolderName"></param>
    /// <returns></returns>
    private string GetFullPathForLocalItem(string localFolderName)
    {
        var dirPath = PathUtils.CombinePath(_currentPath, localFolderName);

        if (dirPath.EndsWith('/')) dirPath = dirPath[..^1];

        return dirPath;
    }

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

        var subFolderList = new List<FolderModel>();
        foreach (var subDirectoryName in dirEntries.SelectMany(de => de.GetSubDirectoryNames()).Distinct())
        {
            var subFolderPath = GetFullPathForLocalItem(subDirectoryName);
            var subFolder = UberFileSystem.Instance.GetDirectoryEntries(subFolderPath);
            if (subFolder.Count == 0)
            {
                Logger.Instance.Error($"Could not find folder '{subDirectoryName}' in '{_currentPath}'");
                continue;
            }

            subFolderList.Add(new FolderModel
            {
                Name = subDirectoryName,
                SubEntryCount = subFolder.SelectMany(sf => sf.GetFiles().Concat(sf.GetSubDirectoryNames())).Distinct()
                    .Count()
            });
        }

        FolderListView.SetObjects(subFolderList);
        FolderListView.SelectedIndex = -1;

        var subFileList = new List<FileModel>();
        foreach (var subFileName in dirEntries.SelectMany(de => de.GetFiles()).Distinct())
        {
            var subFilePath = GetFullPathForLocalItem(subFileName);
            var subFile = UberFileSystem.Instance.GetFileEntries(subFilePath);
            if (subFile.Count == 0)
            {
                Logger.Instance.Error($"Could not find file '{subFileName}' in '{_currentPath}'");
                continue;
            }

            var source = subFile.LastOrDefault().Entry.GetArchiveFile().GetName();

            if (subFile.Count > 1) source += $" (+{subFile.Count - 1})";
            subFileList.Add(new FileModel
            {
                Name = subFileName,
                Source = source,
                Size = subFile.LastOrDefault().Entry.Size,
                FileEntries = subFile
            });
        }

        FileListView.SetObjects(subFileList);
        FileListView.SelectedIndex = -1;

        UpdateCurrentPathUi();

        return true;
    }

    private void SetStatusMessage(string message)
    {
        var date = DateTime.Now;
        var hour = date.Hour.ToString().PadLeft(2, '0');
        var minute = date.Minute.ToString().PadLeft(2, '0');
        var second = date.Second.ToString().PadLeft(2, '0');

        StatusLabel.Text = $"[{hour}:{minute}:{second}] {message}";
    }

    private void AddToRecentHistory(string path)
    {
        if (SettingsManager.Instance.Settings.RecentPaths.Contains(path))
            SettingsManager.Instance.Settings.RecentPaths.Remove(path);

        SettingsManager.Instance.Settings.RecentPaths.Insert(0, path);

        SettingsManager.Instance.Settings.RecentPaths = SettingsManager.Instance.Settings.RecentPaths.Take(10).ToList();

        SettingsManager.Instance.Save();
    }

    private void Reset()
    {
        UberFileSystem.Instance.Reset();
        FolderListView.ClearObjects();
        FileListView.ClearObjects();
        _currentPath = "";
        UpdateCurrentPathUi();
    }

    private bool CheckIfLoading()
    {
        if (_isLoading) MessageBox.Show("Archive files are still being loaded, please wait.");
        return _isLoading;
    }

    internal void OpenArchiveFile(string path)
    {
        if (CheckIfLoading()) return;

        if (_isExtracting)
        {
            MessageBox.Show("Cannot open an archive file while an extraction is running");
            return;
        }

        Reset();

        UberFileSystem.Instance.AddSourceFile(path);

        Logger.Instance.Info($"Opened File: {path}");
        SetStatusMessage($"Opened File: {path}");
        Text = $"ScsFileBrowser - {path}";
        ChangePath("");
        AddToRecentHistory(path);
    }

    internal void OpenArchiveFolder(string path)
    {
        if (CheckIfLoading()) return;

        if (_isExtracting)
        {
            MessageBox.Show("Cannot open a folder while an extraction is running");
            return;
        }

        Reset();
        ShowBusyIndicator("Loading archive files");
        _isLoading = true;
        Task.Run(() => UberFileSystem.Instance.AddSourceDirectory(path))
            .ContinueWith(t =>
            {
                _isLoading = false;
                if (!t.IsCompletedSuccessfully)
                {
                    Invoke(() =>
                    {
                        SetStatusMessage($"Error opening '{path}'");
                        Text = "ScsFileBrowser";
                        RemoveBusyIndicator();
                    });
                    Logger.Instance.Error($"Could not load directory: {t.Exception}");
                    return;
                }

                Logger.Instance.Info($"Opened Folder: {path}");
                AddToRecentHistory(path);
                Invoke(() =>
                {
                    SetStatusMessage($"Opened Folder: {path}");
                    Text = $"ScsFileBrowser - {path}";
                    ChangePath("");
                    RemoveBusyIndicator();
                });
            });
    }

    private string? ExtractFile(UberFile file, string extractionFolder, string originalPath,
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

    private void ExtractFolder(string path, string extractionFolder)
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

    private void ExtractFoldersPrompt(IReadOnlyList<string> paths)
    {
        if (_isExtracting)
        {
            MessageBox.Show("Cannot have multiple extractions running at once.");
            return;
        }

        using var folderBrowserDialog = new FolderBrowserDialog
        {
            InitialDirectory = SettingsManager.Instance.Settings.LastExportPath,
            SelectedPath = SettingsManager.Instance.Settings.LastExportPath
        };

        if (folderBrowserDialog.ShowDialog() != DialogResult.OK) return;

        SettingsManager.Instance.Settings.LastExportPath = folderBrowserDialog.SelectedPath;
        SettingsManager.Instance.Save();

        var statusMsg = $"Extracting folder(s) {string.Join(", ", paths)}";
        SetStatusMessage(statusMsg);
        Logger.Instance.Debug(statusMsg);

        _isExtracting = true;
        ShowBusyIndicator("Extracting folder");
        Task.Run(() =>
        {
            foreach (var path in paths)
            {
                ExtractFolder(path, SettingsManager.Instance.Settings.LastExportPath);
                Logger.Instance.Info($"Finished extracting folder '{path}'");
            }
        }).ContinueWith(_ =>
        {
            Logger.Instance.Info($"Finished extracting folder(s) {string.Join(", ", paths)}");
            Invoke(() =>
            {
                SetStatusMessage($"Finished extracting folder(s) {string.Join(", ", paths)}");
                RemoveBusyIndicator();
            });
            _isExtracting = false;
        });
    }

    private void CopyFolderContentAsText(IReadOnlyList<string> paths)
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

        Clipboard.SetText(sb.ToString());

        SetStatusMessage("Copied folder contents to clipboard");
    }

    // Applications that can open files should be able to take a file path as an argument
    private void OpenFileWithProgram(Settings.Program program, string path)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = program.Path,
            Arguments = $"\"{path}\""
        };
        Process.Start(startInfo);
    }

    private void OpenFile(string path, Settings.Program program)
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

        var fileOutPath = ExtractFile(file.FirstOrDefault(), Consts.TempFileDirPath, path);
        if (fileOutPath == null) return;
        OpenFileWithProgram(program, fileOutPath);
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
            SetStatusMessage($"Could not find program '{program}'");
            Logger.Instance.Error($"Could not find program '{program}'");
            return;
        }

        OpenFile(path, program);
    }

    private void OpenFiles(List<string> paths)
    {
        foreach (var path in paths) OpenFile(path);
    }

    private void OpenFiles(List<string> paths, Settings.Program program)
    {
        foreach (var path in paths) OpenFile(path, program);
    }

    private void ExtractFilesPrompt(List<string> paths)
    {
        if (_isExtracting)
        {
            MessageBox.Show("Cannot have multiple extractions running at once.");
            return;
        }

        using var folderBrowserDialog = new FolderBrowserDialog
        {
            InitialDirectory = SettingsManager.Instance.Settings.LastExportPath,
            SelectedPath = SettingsManager.Instance.Settings.LastExportPath
        };

        if (folderBrowserDialog.ShowDialog() != DialogResult.OK) return;

        SetStatusMessage("Extracting files...");
        Logger.Instance.Debug($"Extracting files ({string.Join(", ", paths)})");

        SettingsManager.Instance.Settings.LastExportPath = folderBrowserDialog.SelectedPath;
        SettingsManager.Instance.Save();

        _isExtracting = true;
        ShowBusyIndicator("Extracting files");
        Task.Run(() =>
        {
            var extractedCount = 0;
            foreach (var filePath in paths)
            {
                var file = UberFileSystem.Instance.GetFileEntries(filePath).LastOrDefault();
                if (file == null)
                {
                    Logger.Instance.Error($"Could not find file '{filePath}'");
                    continue;
                }

                if (ExtractFile(file, SettingsManager.Instance.Settings.LastExportPath, filePath, filePath) != null)
                    extractedCount++;
            }

            return extractedCount;
        }).ContinueWith(task =>
        {
            _isExtracting = false;
            Invoke(RemoveBusyIndicator);
            if (!task.IsCompletedSuccessfully)
            {
                Logger.Instance.Error("Extraction task failed to complete");
                return;
            }

            Invoke(() => SetStatusMessage($"Finished extracting {task.Result} file(s)."));
            Logger.Instance.Info($"Finished extracting {task.Result} file(s).");
        });
    }

    private void ShowBusyIndicator(string labelText)
    {
        BusySpinner.Start();
        BusySpinner.Visible = true;
        BusyLabel.Text = labelText;
        BusyLabel.Visible = true;
    }

    private void RemoveBusyIndicator()
    {
        BusySpinner.Stop();
        BusySpinner.Visible = false;
        BusyLabel.Visible = false;
    }

    #region Events

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (CurrentPathFlowPanel.InEditMode() || keyData != Keys.Back) return base.ProcessCmdKey(ref msg, keyData);
        // Go back on dir when backspace pressed when not in edit mode
        var newPath = "";
        if (_currentPath.Contains('/'))
        {
            var lastSlash = _currentPath.LastIndexOf('/');
            newPath = _currentPath[..lastSlash];
        }

        ChangePath(newPath);
        return true;
    }

    private void SettingsMenuBtn_Click(object sender, EventArgs e)
    {
        using var settingsForm = new SettingsForm();
        settingsForm.ShowDialog();
    }

    private void CurrentPathFlowPanel_PathChanged(object? sender, PathSelectorFlowLayoutPanel.PathChangedEventArgs e)
    {
        if (CheckIfLoading()) return;
        if (!ChangePath(e.NewPath))
        {
            Logger.Instance.Debug($"The path '{e.NewPath}' was not found in the current archive(s)");
            HelpToolTip.Show("The selected path was not found in the archive(s).", CurrentPathFlowPanel, 20, 20, 2000);
        }

        UpdateCurrentPathUi();
    }

    private void FileContextMenuStrip_Opening(object sender, CancelEventArgs e)
    {
        for (var i = OpenRecentToolStripMenuItem.DropDownItems.Count - 1; i >= 0; i--)
            OpenRecentToolStripMenuItem.DropDownItems[i].Dispose();

        if (SettingsManager.Instance.Settings.RecentPaths.Count == 0)
        {
            OpenRecentToolStripMenuItem.Enabled = false;
            return;
        }

        OpenRecentToolStripMenuItem.Enabled = true;

        foreach (var settingsRecentPath in SettingsManager.Instance.Settings.RecentPaths)
        {
            var menuItem = new ThemedToolStripMenuItem
            {
                Text = settingsRecentPath,
                ForeColor = ThemedColors.Text
            };
            menuItem.Click += RecentMenuItem_Click;
            OpenRecentToolStripMenuItem.DropDownItems.Add(menuItem);
        }
    }

    private void RecentMenuItem_Click(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem toolstrip) return;

        if (!File.Exists(toolstrip.Text) && !Directory.Exists(toolstrip.Text))
        {
            SetStatusMessage($"Could not open the recent path '{toolstrip.Text}'");
            Logger.Instance.Error($"Could not open the recent path '{toolstrip.Text}'");
            return;
        }

        Logger.Instance.Debug($"Opening recent path '{toolstrip.Text}'");
        var attr = File.GetAttributes(toolstrip.Text);
        if (attr.HasFlag(FileAttributes.Directory))
            OpenArchiveFolder(toolstrip.Text);
        else
            OpenArchiveFile(toolstrip.Text);
    }

    private void FolderListView_ItemActivate(object sender, EventArgs e)
    {
        if (CheckIfLoading()) return;
        if (FolderListView.SelectedObject is not FolderModel selectedFolder) return;
        ChangePath(GetFullPathForLocalItem(selectedFolder.Name));
    }

    private void FileListView_ItemActivate(object sender, EventArgs e)
    {
        if (CheckIfLoading()) return;
        if (_isExtracting)
        {
            MessageBox.Show("Cannot open file(s) when an extraction is running.");
            return;
        }

        if (FileListView.SelectedObject is not FileModel selectedItem) return;
        var fileName = selectedItem.Name;

        OpenFile(GetFullPathForLocalItem(fileName));
    }

    private void FileListView_ItemDrag(object sender, ItemDragEventArgs e)
    {
        if (CheckIfLoading()) return;
        if (_isExtracting)
        {
            MessageBox.Show("Cannot extract file(s) when an extraction is already running.");
            return;
        }

        if (FileListView.SelectedObjects.Count == 0) return;
        var filePaths = new StringCollection();

        DataObject dataObject = new();
        foreach (var selectedItem in FileListView.SelectedObjects)
        {
            if (selectedItem is not FileModel item) continue;
            var filePath = GetFullPathForLocalItem(item.Name);
            var file = UberFileSystem.Instance.GetFileEntries(filePath).LastOrDefault();

            if (file == null)
            {
                Logger.Instance.Error($"Could not extract file '{filePath}'");
                continue;
            }

            var fp = ExtractFile(file, Consts.TempFileDirPath, filePath, filePath);
            if (fp != null)
                filePaths.Add(fp);
        }

        if (filePaths.Count == 0) return;
        Logger.Instance.Debug(
            $"Drag dropping files: {string.Join(", ", FileListView.SelectedObjects.Cast<FileModel>().Select(f => GetFullPathForLocalItem(f.Name)))}");
        dataObject.SetFileDropList(filePaths);
        FileListView.DoDragDrop(dataObject, DragDropEffects.Copy);
    }

    private void OpenFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        BrowseFileDialog.InitialDirectory = Path.GetDirectoryName(SettingsManager.Instance.Settings.LastSelectedFile);
        BrowseFileDialog.FileName = SettingsManager.Instance.Settings.LastSelectedFile;
        if (BrowseFileDialog.ShowDialog() != DialogResult.OK) return;

        SettingsManager.Instance.Settings.LastSelectedFile = BrowseFileDialog.FileName;
        SettingsManager.Instance.Save();

        OpenArchiveFile(BrowseFileDialog.FileName);
    }

    private void OpenFolderToolStripMenuItem_Click(object sender, EventArgs e)
    {
        BrowseFolderDialog.InitialDirectory = SettingsManager.Instance.Settings.LastSelectedFolder;
        BrowseFolderDialog.SelectedPath = SettingsManager.Instance.Settings.LastSelectedFolder;
        if (BrowseFolderDialog.ShowDialog() != DialogResult.OK) return;

        SettingsManager.Instance.Settings.LastSelectedFolder = BrowseFolderDialog.SelectedPath;
        SettingsManager.Instance.Save();

        OpenArchiveFolder(BrowseFolderDialog.SelectedPath);
    }

    private void FolderListView_CellRightClick(object sender, CellRightClickEventArgs e)
    {
        FolderListView.ContextMenuStrip?.Dispose();
        if (CheckIfLoading()) return;
        var selectedPaths = new List<string>();
        if (FolderListView.SelectedIndex == -1 /* && _currentFolder != null && _currentFolder.GetSubEntryCount() != 0*/)
            selectedPaths.Add(_currentPath);
        else
        {
            var selectedItems = FolderListView.SelectedObjects;

            foreach (var selectedItem in selectedItems)
            {
                if (selectedItem is not FolderModel folder) continue;
                selectedPaths.Add(GetFullPathForLocalItem(folder.Name));
            }

            if (selectedPaths.Count == 0) return;
        }

        var hasMultiple = selectedPaths.Count > 1;

        var contextMenu = new ContextMenuStrip
        {
            Renderer = new ThemedToolStripMenuRenderer(),
            ShowImageMargin = false,
            BackColor = ThemedColors.Base
        };

        var copyPathMenuItem = new ThemedToolStripMenuItem
        {
            Text = hasMultiple ? "Copy Paths" : "Copy Path"
        };
        copyPathMenuItem.Click += (_, _) => { CommonUtils.TextToClipboard(string.Join("\n", selectedPaths)); };

        contextMenu.Items.Add(copyPathMenuItem);

        if (e.ModifierKeys == Keys.Shift)
        {
            if (!_isExtracting)
            {
                var subItemsAsTextToClipboardMenuItem = new ThemedToolStripMenuItem
                {
                    Text = "Copy sub folder/file names",
                    ToolTipText =
                        $"Copies the sub folder and file names for the {(hasMultiple ? "directories" : "directory")} to the clipboard"
                };
                subItemsAsTextToClipboardMenuItem.Click += (_, _) => { CopyFolderContentAsText(selectedPaths); };

                contextMenu.Items.Add(subItemsAsTextToClipboardMenuItem);
            }

            var copyCityHashMenuItem = new ThemedToolStripMenuItem
            {
                Text = "Copy CityHash Value",
                ToolTipText =
                    $"Copies the cityhash value for the {(hasMultiple ? "directories" : "directory")} to the clipboard"
            };
            copyCityHashMenuItem.Click += (_, _) =>
            {
                CommonUtils.TextToClipboard(string.Join("\n",
                    selectedPaths.Select(p => $"{CityHash.CityHash64(p):X}")));
            };

            contextMenu.Items.Add(copyCityHashMenuItem);
        }

        if (!_isExtracting)
        {
            var extractFolderMenuItem = new ThemedToolStripMenuItem
            {
                Text = hasMultiple ? "Extract Folders" : "Extract Folder",
                ToolTipText =
                    $"Extracts the {(hasMultiple ? "directories" : "directory")} with the current path '{_currentPath}' prefixed"
            };
            extractFolderMenuItem.Click += (_, _) => { ExtractFoldersPrompt(selectedPaths); };

            contextMenu.Items.Add(extractFolderMenuItem);
        }

        FolderListView.ContextMenuStrip = contextMenu;
        FolderListView.ContextMenuStrip.Show(e.Location);
    }

    private void FileListView_CellRightClick(object sender, CellRightClickEventArgs e)
    {
        FileListView.ContextMenuStrip?.Dispose();
        if (CheckIfLoading()) return;
        var selectedItems = FileListView.SelectedObjects;

        var selectedPaths = new List<string>();
        foreach (var selectedItem in selectedItems)
        {
            if (selectedItem is not FileModel file) continue;
            selectedPaths.Add(GetFullPathForLocalItem(file.Name));
        }

        if (selectedPaths.Count == 0) return;

        var hasMultiple = selectedPaths.Count > 1;

        var contextMenu = new ContextMenuStrip
        {
            Renderer = new ThemedToolStripMenuRenderer(),
            ShowImageMargin = false,
            BackColor = ThemedColors.Base
        };
        if (!_isExtracting)
        {
            var openFileMenuItem = new ThemedToolStripMenuItem
            {
                Text = "Open"
            };
            openFileMenuItem.Click += (_, _) => { OpenFiles(selectedPaths); };
            contextMenu.Items.Add(openFileMenuItem);

            if (SettingsManager.Instance.Settings.Programs.Count > 0)
            {
                var openFileWithMenuItem = new ThemedToolStripMenuItem
                {
                    Text = "Open With",
                    DropDown = new ContextMenuStrip
                    {
                        Renderer = new ThemedToolStripMenuRenderer(),
                        ShowImageMargin = false,
                        BackColor = ThemedColors.Base
                    }
                };
                foreach (var program in SettingsManager.Instance.Settings.Programs)
                {
                    var programMenuItem = new ThemedToolStripMenuItem
                    {
                        Text = program.Name
                    };
                    programMenuItem.Click += (_, _) => { OpenFiles(selectedPaths, program); };
                    openFileWithMenuItem.DropDownItems.Add(programMenuItem);
                }


                contextMenu.Items.Add(openFileWithMenuItem);
            }
        }

        var copyPathMenuItem = new ThemedToolStripMenuItem
        {
            Text = hasMultiple ? "Copy Paths" : "Copy Path"
        };
        copyPathMenuItem.Click += (_, _) =>
        {
            CommonUtils.TextToClipboard(string.Join("\n", selectedPaths.Select(p => $"/{p}")));
        };

        contextMenu.Items.Add(copyPathMenuItem);

        if (e.ModifierKeys == Keys.Shift)
        {
            var copyCityHashMenuItem = new ThemedToolStripMenuItem
            {
                Text = "Copy CityHash Value",
                ToolTipText =
                    $"Copies the cityhash value for the {(hasMultiple ? "files" : "file")} to the clipboard"
            };
            copyCityHashMenuItem.Click += (_, _) =>
            {
                CommonUtils.TextToClipboard(string.Join("\n",
                    selectedPaths.Select(p => $"{CityHash.CityHash64(p):X}")));
            };

            contextMenu.Items.Add(copyCityHashMenuItem);
        }

        if (!_isExtracting)
        {
            var extractFileAbsMenuItem = new ThemedToolStripMenuItem
            {
                Text = hasMultiple ? "Extract Files" : "Extract File",
                ToolTipText =
                    $"Extracts the {(hasMultiple ? "files" : "file")} with the current path '{_currentPath}' prefixed"
            };
            extractFileAbsMenuItem.Click += (_, _) => { ExtractFilesPrompt(selectedPaths); };

            contextMenu.Items.Add(extractFileAbsMenuItem);
        }

        FileListView.ContextMenuStrip = contextMenu;
        FileListView.ContextMenuStrip.Show(e.Location);
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_isExtracting &&
            MessageBox.Show("An extraction is still running, are you sure you want to quit?",
                "Stop extraction and close?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
            DialogResult.No) e.Cancel = true;


        if (e.CloseReason == CloseReason.WindowsShutDown)
            return; // Stop program from preventing Windows from shutting down

        if (!Directory.Exists(Consts.TempFileDirPath)) return;
        var subDirs = Directory.GetDirectories(Consts.TempFileDirPath);
        var subFiles = Directory.GetFiles(Consts.TempFileDirPath);
        if (subDirs.Length == 0 && subFiles.Length == 0) return;

        if (MessageBox.Show("Cleanup temporary files?", "Do you want to remove all the temporarily extracted files?",
                MessageBoxButtons.YesNo) == DialogResult.No) return;

        if (!Directory.Exists(Consts.TempFileDirPath)) return;

        foreach (var subFile in subFiles) File.Delete(subFile);

        foreach (var subDir in subDirs) Directory.Delete(subDir, true);
    }

    private void MainForm_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is not string[] {Length: > 0} files) return;

        var filePath = files[0];
        if (Consts.AllowedDropExtensions.Contains(Path.GetExtension(filePath)))
            OpenArchiveFile(filePath);
        else
        {
            var err =
                $"File '{filePath}' did not have an allowed file extension ({string.Join(", ", Consts.AllowedDropExtensions)})";
            Logger.Instance.Error(err);
            MessageBox.Show(err, "Invalid file type", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void MainForm_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data == null || _isExtracting || _isLoading) return;
        if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Link;
    }

    #endregion
}
