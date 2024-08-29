using BrightIdeasSoftware;
using ScsFileBrowser.Logging;
using ScsFileBrowser.Settings;
using ScsFileBrowser.UI.Components;
using ScsFileBrowser.UI.Dialogs;

namespace ScsFileBrowser.UI.Forms;

public partial class SettingsForm : Form
{
    private const string HelpText = "- Drag an extension to a program to assign it to the program.\n" +
                                    "- Right click on an extension or program for options to rename or delete it.";

    private bool _hasChangesPending;

    public SettingsForm()
    {
        InitializeComponent();
        SetTheme();
        UpdateList();
        SettingsTreeView.ExpandAll();
    }

    private void SetTheme()
    {
        BackColor = ThemedColors.Surface;
        ForeColor = ThemedColors.Text;

        HelpToolTip.BackColor = ThemedColors.Base;
        HelpLabel.ForeColor = ThemedColors.Gold;

        SettingsTreeView.BackColor = ThemedColors.Overlay;
        SettingsTreeView.ForeColor = ThemedColors.Text;

        SettingsTreeView.SelectedBackColor = ThemedColors.Pine;
        SettingsTreeView.UnfocusedSelectedBackColor = ThemedColors.HighlightLow;
        SettingsTreeView.UnfocusedSelectedForeColor = ThemedColors.Subtle;

        SettingsTreeView.TreeColumnRenderer.LinePen = new Pen(ThemedColors.Pine);

        SettingsTreeView.HeaderMaximumHeight = 0; // hide useless header

        SettingsTreeView.EmptyListMsgOverlay = new TextOverlay
        {
            TextColor = ThemedColors.Foam,
            Alignment = ContentAlignment.MiddleCenter
        };
        SettingsTreeView.EmptyListMsg =
            "Once you add programs and extensions you will be able to see and configure them here.";
        HelpToolTip.SetToolTip(HelpLabel, HelpText);

        var sink = (SimpleDropSink) SettingsTreeView.DropSink;
        sink.AcceptExternal = false;
        sink.CanDropOnSubItem = false;
        sink.CanDropOnBackground = false;

        SettingsTreeView.Columns.Add(new OLVColumn
        {
            Text = "Program",
            FillsFreeSpace = true,
            AspectName = "Name"
        });

        SettingsTreeView.CanExpandGetter = model => model is Settings.Program;
        SettingsTreeView.ChildrenGetter = delegate(object model)
        {
            return SettingsManager.Instance.Settings.Extensions.Where(e =>
                e.DefaultProgram == ((Settings.Program) model).Name).OrderBy(e => e.Name);
        };
    }

    private void UpdateList()
    {
        SettingsTreeView.Roots = SettingsManager.Instance.Settings.Programs.OrderBy(p => p.Name);
    }

    private void RenameProgram(Settings.Program program)
    {
        using var inputDialog = new TextInputDialog($"Renaming '${program.Name}'", "Enter a new program name",
            program.Name);

        if (inputDialog.ShowDialog() != DialogResult.OK
            || string.IsNullOrWhiteSpace(inputDialog.TextValue)
            || program.Name == inputDialog.TextValue) return;

        var newProgramName = inputDialog.TextValue;

        Logger.Instance.Debug($"Renaming program from '{program.Name}' to '{newProgramName}'");

        SettingsManager.Instance.Settings.Extensions.ForEach(ex =>
        {
            if (ex.DefaultProgram != program.Name) return;
            ex.DefaultProgram = newProgramName;
            Logger.Instance.Debug(
                $"Renamed default program for '{ex.Name}' from '{program.Name}' to '{newProgramName}'");
        });
        program.Name = newProgramName;
        _hasChangesPending = true;
    }

    private void RenameExtension(Extension extension)
    {
        using var inputDialog = new TextInputDialog($"Renaming '${extension.Name}'",
            "Enter a new extension name",
            extension.Name);

        if (inputDialog.ShowDialog() != DialogResult.OK
            || string.IsNullOrWhiteSpace(inputDialog.TextValue)
            || extension.Name == inputDialog.TextValue) return;

        Logger.Instance.Debug($"Renaming extension from '{extension.Name}' to '{inputDialog.TextValue}'");

        extension.Name = inputDialog.TextValue;
        _hasChangesPending = true;
    }

    private void DeleteProgram(Settings.Program program)
    {
        if (SettingsManager.Instance.Settings.Extensions.Any(ex => ex.DefaultProgram == program.Name))
        {
            MessageBox.Show("Cannot remove a program that has extension(s) assigned to it.",
                "Could not delete program.",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var result = MessageBox.Show($"Are you sure you want to delete the program '{program.Name}'?",
            "Delete program?",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (result != DialogResult.Yes) return;

        Logger.Instance.Debug($"Deleting program '{program.Name}'");

        SettingsManager.Instance.Settings.Programs.Remove(program);
        _hasChangesPending = true;
    }

    private void DeleteExtension(Extension extension)
    {
        var result = MessageBox.Show($"Are you sure you want to delete the extension '{extension.Name}'?",
            "Delete extension?",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (result != DialogResult.Yes) return;

        Logger.Instance.Debug($"Deleting extension '{extension.Name}'");

        SettingsManager.Instance.Settings.Extensions.Remove(extension);
        _hasChangesPending = true;
    }

    private void HelpLabel_Click(object sender, EventArgs e)
    {
        MessageBox.Show(HelpText, "Settings Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    #region Events

    private void SettingsTreeView_ModelCanDrop(object sender, ModelDropEventArgs e)
    {
        var sink = e.DropSink;
        sink.FeedbackColor = Color.Transparent;
        sink.Billboard.BackColor = ThemedColors.Base;
        sink.Billboard.TextColor = ThemedColors.Foam;
        if (e.SourceModels.Cast<object?>().Any(sourceModel => sourceModel is not Extension))
        {
            e.Effect = DragDropEffects.None;
            e.InfoMessage = "You can only assign extensions to programs";
            sink.Billboard.TextColor = ThemedColors.Love;
            return;
        }

        if (e.TargetModel is not Settings.Program program)
        {
            e.Effect = DragDropEffects.None;
            e.InfoMessage = "Cannot assign an extension to another extension";
            sink.Billboard.TextColor = ThemedColors.Love;
            return;
        }

        e.InfoMessage =
            $"Sets '{program.Name}' as the default program for '{string.Join(", ", e.SourceModels.Cast<Extension>())}'";

        sink.FeedbackColor = ThemedColors.Pine;
        e.Effect = DragDropEffects.Move;
    }

    private void SettingsTreeView_ModelDropped(object sender, ModelDropEventArgs e)
    {
        // Should always be program because of `ModelCanDrop`, but just double checking to be sure
        if (e.TargetModel is not Settings.Program targetProgram)
        {
            e.Effect = DragDropEffects.None;
            return;
        }

        foreach (Extension extensionToMove in e.SourceModels)
        {
            Logger.Instance.Debug(
                $"Setting default program for extension '{extensionToMove.Name}' from '{extensionToMove.DefaultProgram}' to '{targetProgram.Name}'");
            extensionToMove.DefaultProgram = targetProgram.Name;
        }

        UpdateList();
        _hasChangesPending = true;
    }

    private void AddProgramBtn_Click(object sender, EventArgs e)
    {
        if (BrowseProgramDialog.ShowDialog() != DialogResult.OK) return;

        var programName = Path.GetFileNameWithoutExtension(BrowseProgramDialog.FileName);

        using var inputDialog = new TextInputDialog("Add a program", "Enter a name for the program", programName);

        if (inputDialog.ShowDialog() != DialogResult.OK) return;

        if (!string.IsNullOrEmpty(inputDialog.TextValue))
            programName = inputDialog.TextValue;

        if (SettingsManager.Instance.Settings.Programs.Any(p => p.Path == BrowseProgramDialog.FileName)) return;

        Logger.Instance.Debug($"Adding new program '{programName}' with path '{BrowseProgramDialog.FileName}'");

        SettingsManager.Instance.Settings.Programs.Add(new Settings.Program
        {
            Path = BrowseProgramDialog.FileName,
            Name = programName
        });
        SettingsManager.Instance.Save();
        UpdateList();
    }

    private void AddExtensionBtn_Click(object sender, EventArgs e)
    {
        if (SettingsManager.Instance.Settings.Programs.Count == 0)
        {
            MessageBox.Show("At least one program is required to add an extension");
            return;
        }

        using var inputDialog = new ComboTextInputDialog("Add a file extension", "Enter a file extension (e.g. .sii)",
            "Select a default program for the extension",
            SettingsManager.Instance.Settings.Programs.Select(p => p.Name).ToList(), null,
            SettingsTreeView.SelectedObject is Settings.Program program ? program.Name : null);

        if (inputDialog.ShowDialog() != DialogResult.OK
            || string.IsNullOrWhiteSpace(inputDialog.ComboValue)
            || string.IsNullOrWhiteSpace(inputDialog.TextValue)) return;
        var extension = inputDialog.TextValue;

        if (!extension.StartsWith('.')) extension = $".{extension}";
        if (SettingsManager.Instance.Settings.Extensions.Any(x => x.Name == extension)) return;

        Logger.Instance.Debug($"Adding new extension '{extension}' with default program '{inputDialog.ComboValue}'");

        SettingsManager.Instance.Settings.Extensions.Add(new Extension
            {Name = extension, DefaultProgram = inputDialog.ComboValue});

        SettingsManager.Instance.Save();
        UpdateList();
    }

    private void SaveSettingsBtn_Click(object sender, EventArgs e)
    {
        _hasChangesPending = false;
        SettingsManager.Instance.Save();
    }

    private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!_hasChangesPending) return;

        var result = MessageBox.Show("There are unsaved changes, do you want to save them?",
            "Save changes?",
            MessageBoxButtons.YesNoCancel);
        if (result == DialogResult.Yes)
            SettingsManager.Instance.Save();
        else if (result == DialogResult.Cancel) e.Cancel = true;
        else if (result == DialogResult.No) SettingsManager.Instance.Load();
    }

    private void SettingsTreeView_CellRightClick(object sender, CellRightClickEventArgs e)
    {
        SettingsTreeView.ContextMenuStrip?.Dispose();
        if (SettingsTreeView.SelectedObjects.Count > 1)
        {
            MessageBox.Show("You can only modify one item at a time");
            return;
        }

        if (SettingsTreeView.SelectedObjects.Count == 0) return;

        var contextMenu = new ContextMenuStrip
        {
            Renderer = new ThemedToolStripMenuRenderer(),
            ShowImageMargin = false,
            BackColor = ThemedColors.Base
        };
        var renameMenuItem = new ThemedToolStripMenuItem
        {
            Text = "Rename"
        };
        renameMenuItem.Click += (_, _) =>
        {
            if (SettingsTreeView.SelectedObject is Settings.Program program)
                RenameProgram(program);
            else if (SettingsTreeView.SelectedObject is Extension extension) RenameExtension(extension);

            UpdateList();
        };
        contextMenu.Items.Add(renameMenuItem);

        var deleteMenuItem = new ThemedToolStripMenuItem
        {
            Text = "Delete",
            ForeColor = ThemedColors.Love
        };
        contextMenu.Items.Add(deleteMenuItem);
        deleteMenuItem.Click += (_, _) =>
        {
            if (SettingsTreeView.SelectedObject is Settings.Program program)
                DeleteProgram(program);
            else if (SettingsTreeView.SelectedObject is Extension extension) DeleteExtension(extension);

            UpdateList();
        };

        SettingsTreeView.ContextMenuStrip = contextMenu;
        SettingsTreeView.ContextMenuStrip.Show(e.Location);
    }

    #endregion
}
