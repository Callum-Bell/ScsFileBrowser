using ScsFileBrowser.UI.Components;

namespace ScsFileBrowser.UI.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.FileContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.OpenFileToolStripMenuItem = new ScsFileBrowser.UI.Components.ThemedToolStripMenuItem();
            this.OpenFolderToolStripMenuItem = new ScsFileBrowser.UI.Components.ThemedToolStripMenuItem();
            this.OpenRecentToolStripMenuItem = new ScsFileBrowser.UI.Components.ThemedToolStripMenuItem();
            this.FileMenuBtn = new ScsFileBrowser.UI.Components.ThemedDropButton();
            this.SettingsMenuBtn = new ScsFileBrowser.UI.Components.ThemedDropButton();
            this.CurrentPathLbl = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.FolderListView = new BrightIdeasSoftware.FastObjectListView();
            this.FileListView = new BrightIdeasSoftware.FastObjectListView();
            this.CurrentPathFlowPanel = new ScsFileBrowser.UI.Components.PathSelectorFlowLayoutPanel();
            this.BrowseFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.BrowseFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.HelpToolTip = new ScsFileBrowser.UI.Components.ThemedToolTip(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TopPanel = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.BusyLabel = new System.Windows.Forms.Label();
            this.BusySpinner = new ScsFileBrowser.UI.Components.Spinner();
            this.FileContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FolderListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FileListView)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.TopPanel.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FileContextMenuStrip
            // 
            this.FileContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenFileToolStripMenuItem,
            this.OpenFolderToolStripMenuItem,
            this.OpenRecentToolStripMenuItem});
            this.FileContextMenuStrip.Name = "contextMenuStrip1";
            this.FileContextMenuStrip.ShowImageMargin = false;
            this.FileContextMenuStrip.Size = new System.Drawing.Size(118, 70);
            this.FileContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.FileContextMenuStrip_Opening);
            // 
            // OpenFileToolStripMenuItem
            // 
            this.OpenFileToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(23)))), ((int)(((byte)(36)))));
            this.OpenFileToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(222)))), ((int)(((byte)(244)))));
            this.OpenFileToolStripMenuItem.Name = "OpenFileToolStripMenuItem";
            this.OpenFileToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.OpenFileToolStripMenuItem.Text = "Open File";
            this.OpenFileToolStripMenuItem.Click += new System.EventHandler(this.OpenFileToolStripMenuItem_Click);
            // 
            // OpenFolderToolStripMenuItem
            // 
            this.OpenFolderToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(23)))), ((int)(((byte)(36)))));
            this.OpenFolderToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(222)))), ((int)(((byte)(244)))));
            this.OpenFolderToolStripMenuItem.Name = "OpenFolderToolStripMenuItem";
            this.OpenFolderToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.OpenFolderToolStripMenuItem.Text = "Open Folder";
            this.OpenFolderToolStripMenuItem.Click += new System.EventHandler(this.OpenFolderToolStripMenuItem_Click);
            // 
            // OpenRecentToolStripMenuItem
            // 
            this.OpenRecentToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(23)))), ((int)(((byte)(36)))));
            this.OpenRecentToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(222)))), ((int)(((byte)(244)))));
            this.OpenRecentToolStripMenuItem.Name = "OpenRecentToolStripMenuItem";
            this.OpenRecentToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.OpenRecentToolStripMenuItem.Text = "Open Recent";
            // 
            // FileMenuBtn
            // 
            this.FileMenuBtn.FlatAppearance.BorderSize = 0;
            this.FileMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FileMenuBtn.Location = new System.Drawing.Point(3, 3);
            this.FileMenuBtn.Name = "FileMenuBtn";
            this.FileMenuBtn.Size = new System.Drawing.Size(70, 26);
            this.FileMenuBtn.TabIndex = 1;
            this.FileMenuBtn.Text = "File";
            this.FileMenuBtn.UseVisualStyleBackColor = true;
            // 
            // SettingsMenuBtn
            // 
            this.SettingsMenuBtn.FlatAppearance.BorderSize = 0;
            this.SettingsMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsMenuBtn.Location = new System.Drawing.Point(79, 3);
            this.SettingsMenuBtn.Name = "SettingsMenuBtn";
            this.SettingsMenuBtn.Size = new System.Drawing.Size(70, 26);
            this.SettingsMenuBtn.TabIndex = 1;
            this.SettingsMenuBtn.Text = "Settings";
            this.SettingsMenuBtn.UseVisualStyleBackColor = true;
            this.SettingsMenuBtn.Click += new System.EventHandler(this.SettingsMenuBtn_Click);
            // 
            // CurrentPathLbl
            // 
            this.CurrentPathLbl.AutoSize = true;
            this.CurrentPathLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CurrentPathLbl.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CurrentPathLbl.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.CurrentPathLbl.Location = new System.Drawing.Point(3, 0);
            this.CurrentPathLbl.Name = "CurrentPathLbl";
            this.CurrentPathLbl.Size = new System.Drawing.Size(92, 24);
            this.CurrentPathLbl.TabIndex = 2;
            this.CurrentPathLbl.Text = "Current Path:";
            this.CurrentPathLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 74);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.FolderListView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.FileListView);
            this.splitContainer1.Size = new System.Drawing.Size(812, 563);
            this.splitContainer1.SplitterDistance = 346;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.TabStop = false;
            // 
            // FolderListView
            // 
            this.FolderListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FolderListView.CellEditUseWholeCell = false;
            this.FolderListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FolderListView.FullRowSelect = true;
            this.FolderListView.Location = new System.Drawing.Point(0, 0);
            this.FolderListView.Name = "FolderListView";
            this.FolderListView.ShowGroups = false;
            this.FolderListView.Size = new System.Drawing.Size(346, 563);
            this.FolderListView.TabIndex = 1;
            this.FolderListView.UseHotItem = true;
            this.FolderListView.View = System.Windows.Forms.View.Details;
            this.FolderListView.VirtualMode = true;
            this.FolderListView.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.FolderListView_CellRightClick);
            this.FolderListView.ItemActivate += new System.EventHandler(this.FolderListView_ItemActivate);
            // 
            // FileListView
            // 
            this.FileListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FileListView.CellEditUseWholeCell = false;
            this.FileListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileListView.FullRowSelect = true;
            this.FileListView.Location = new System.Drawing.Point(0, 0);
            this.FileListView.Name = "FileListView";
            this.FileListView.ShowGroups = false;
            this.FileListView.Size = new System.Drawing.Size(460, 563);
            this.FileListView.TabIndex = 2;
            this.FileListView.UseHotItem = true;
            this.FileListView.View = System.Windows.Forms.View.Details;
            this.FileListView.VirtualMode = true;
            this.FileListView.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.FileListView_CellRightClick);
            this.FileListView.ItemActivate += new System.EventHandler(this.FileListView_ItemActivate);
            this.FileListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.FileListView_ItemDrag);
            // 
            // CurrentPathFlowPanel
            // 
            this.CurrentPathFlowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CurrentPathFlowPanel.Location = new System.Drawing.Point(98, 0);
            this.CurrentPathFlowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.CurrentPathFlowPanel.Name = "CurrentPathFlowPanel";
            this.CurrentPathFlowPanel.Size = new System.Drawing.Size(714, 24);
            this.CurrentPathFlowPanel.TabIndex = 7;
            this.CurrentPathFlowPanel.WrapContents = false;
            // 
            // BrowseFolderDialog
            // 
            this.BrowseFolderDialog.ShowNewFolderButton = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 651);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(836, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // HelpToolTip
            // 
            this.HelpToolTip.OwnerDraw = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.CurrentPathLbl, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.CurrentPathFlowPanel, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 44);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(812, 24);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // TopPanel
            // 
            this.TopPanel.ColumnCount = 3;
            this.TopPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TopPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TopPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TopPanel.Controls.Add(this.FileMenuBtn, 0, 0);
            this.TopPanel.Controls.Add(this.SettingsMenuBtn, 1, 0);
            this.TopPanel.Controls.Add(this.flowLayoutPanel1, 2, 0);
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.RowCount = 1;
            this.TopPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TopPanel.Size = new System.Drawing.Size(836, 32);
            this.TopPanel.TabIndex = 10;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.BusyLabel);
            this.flowLayoutPanel1.Controls.Add(this.BusySpinner);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(155, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(678, 26);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // BusyLabel
            // 
            this.BusyLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BusyLabel.AutoSize = true;
            this.BusyLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.BusyLabel.Location = new System.Drawing.Point(675, 0);
            this.BusyLabel.Name = "BusyLabel";
            this.BusyLabel.Size = new System.Drawing.Size(0, 31);
            this.BusyLabel.TabIndex = 0;
            this.BusyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BusyLabel.Visible = false;
            // 
            // BusySpinner
            // 
            this.BusySpinner.Location = new System.Drawing.Point(644, 3);
            this.BusySpinner.Name = "BusySpinner";
            this.BusySpinner.Size = new System.Drawing.Size(25, 25);
            this.BusySpinner.TabIndex = 1;
            this.BusySpinner.Visible = false;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 673);
            this.Controls.Add(this.TopPanel);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(550, 450);
            this.Name = "MainForm";
            this.Text = "ScsFileBrowser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.FileContextMenuStrip.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FolderListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FileListView)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.TopPanel.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ContextMenuStrip FileContextMenuStrip;
        private ThemedDropButton FileMenuBtn;
        private ThemedDropButton SettingsMenuBtn;
        private Label CurrentPathLbl;
        private SplitContainer splitContainer1;
        private BrightIdeasSoftware.FastObjectListView FolderListView;
        private BrightIdeasSoftware.FastObjectListView FileListView;
        private PathSelectorFlowLayoutPanel CurrentPathFlowPanel;
        private ThemedToolStripMenuItem OpenFileToolStripMenuItem;
        private ThemedToolStripMenuItem OpenFolderToolStripMenuItem;
        private ThemedToolStripMenuItem OpenRecentToolStripMenuItem;
        private OpenFileDialog BrowseFileDialog;
        private FolderBrowserDialog BrowseFolderDialog;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel StatusLabel;
        private Components.ThemedToolTip HelpToolTip;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel TopPanel;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label BusyLabel;
        private Spinner BusySpinner;
    }
}