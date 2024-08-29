namespace ScsFileBrowser.UI.Forms
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.SettingsTreeView = new BrightIdeasSoftware.TreeListView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.AddProgramBtn = new ScsFileBrowser.UI.Components.ThemedButton();
            this.AddExtensionBtn = new ScsFileBrowser.UI.Components.ThemedButton();
            this.SaveSettingsBtn = new ScsFileBrowser.UI.Components.ThemedButton();
            this.label1 = new System.Windows.Forms.Label();
            this.BrowseProgramDialog = new System.Windows.Forms.OpenFileDialog();
            this.HelpToolTip = new ScsFileBrowser.UI.Components.ThemedToolTip(this.components);
            this.HelpLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.SettingsTreeView)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SettingsTreeView
            // 
            this.SettingsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SettingsTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SettingsTreeView.CellEditUseWholeCell = false;
            this.SettingsTreeView.IsSimpleDragSource = true;
            this.SettingsTreeView.IsSimpleDropSink = true;
            this.SettingsTreeView.Location = new System.Drawing.Point(12, 30);
            this.SettingsTreeView.Name = "SettingsTreeView";
            this.SettingsTreeView.ShowGroups = false;
            this.SettingsTreeView.Size = new System.Drawing.Size(316, 347);
            this.SettingsTreeView.TabIndex = 0;
            this.SettingsTreeView.View = System.Windows.Forms.View.Details;
            this.SettingsTreeView.VirtualMode = true;
            this.SettingsTreeView.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.SettingsTreeView_CellRightClick);
            this.SettingsTreeView.ModelCanDrop += new System.EventHandler<BrightIdeasSoftware.ModelDropEventArgs>(this.SettingsTreeView_ModelCanDrop);
            this.SettingsTreeView.ModelDropped += new System.EventHandler<BrightIdeasSoftware.ModelDropEventArgs>(this.SettingsTreeView_ModelDropped);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.AddProgramBtn, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.AddExtensionBtn, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.SaveSettingsBtn, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 383);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(340, 37);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // AddProgramBtn
            // 
            this.AddProgramBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddProgramBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(61)))), ((int)(((byte)(82)))));
            this.AddProgramBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddProgramBtn.Location = new System.Drawing.Point(3, 3);
            this.AddProgramBtn.Name = "AddProgramBtn";
            this.AddProgramBtn.Size = new System.Drawing.Size(107, 31);
            this.AddProgramBtn.TabIndex = 0;
            this.AddProgramBtn.Text = "Add Program";
            this.AddProgramBtn.UseVisualStyleBackColor = true;
            this.AddProgramBtn.Click += new System.EventHandler(this.AddProgramBtn_Click);
            // 
            // AddExtensionBtn
            // 
            this.AddExtensionBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddExtensionBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(61)))), ((int)(((byte)(82)))));
            this.AddExtensionBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddExtensionBtn.Location = new System.Drawing.Point(116, 3);
            this.AddExtensionBtn.Name = "AddExtensionBtn";
            this.AddExtensionBtn.Size = new System.Drawing.Size(107, 31);
            this.AddExtensionBtn.TabIndex = 1;
            this.AddExtensionBtn.Text = "Add Extension";
            this.AddExtensionBtn.UseVisualStyleBackColor = true;
            this.AddExtensionBtn.Click += new System.EventHandler(this.AddExtensionBtn_Click);
            // 
            // SaveSettingsBtn
            // 
            this.SaveSettingsBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SaveSettingsBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(61)))), ((int)(((byte)(82)))));
            this.SaveSettingsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveSettingsBtn.Location = new System.Drawing.Point(229, 3);
            this.SaveSettingsBtn.Name = "SaveSettingsBtn";
            this.SaveSettingsBtn.Size = new System.Drawing.Size(108, 31);
            this.SaveSettingsBtn.TabIndex = 2;
            this.SaveSettingsBtn.Text = "Save Settings";
            this.SaveSettingsBtn.UseVisualStyleBackColor = true;
            this.SaveSettingsBtn.Click += new System.EventHandler(this.SaveSettingsBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Programs and their assigned extensions:";
            // 
            // BrowseProgramDialog
            // 
            this.BrowseProgramDialog.Filter = "exe (*.exe)|*.exe";
            // 
            // HelpToolTip
            // 
            this.HelpToolTip.AutoPopDelay = 30000;
            this.HelpToolTip.InitialDelay = 500;
            this.HelpToolTip.OwnerDraw = true;
            this.HelpToolTip.ReshowDelay = 100;
            // 
            // HelpLabel
            // 
            this.HelpLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.HelpLabel.AutoSize = true;
            this.HelpLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.HelpLabel.Location = new System.Drawing.Point(771, 4);
            this.HelpLabel.Name = "HelpLabel";
            this.HelpLabel.Size = new System.Drawing.Size(17, 21);
            this.HelpLabel.TabIndex = 3;
            this.HelpLabel.Text = "?";
            this.HelpLabel.Click += new System.EventHandler(this.HelpLabel_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 420);
            this.Controls.Add(this.HelpLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.SettingsTreeView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(350, 350);
            this.Name = "SettingsForm";
            this.Text = "Settings - ScsFileBrowser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.SettingsTreeView)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.TreeListView SettingsTreeView;
        private TableLayoutPanel tableLayoutPanel1;
        private Components.ThemedButton AddProgramBtn;
        private Components.ThemedButton AddExtensionBtn;
        private Components.ThemedButton SaveSettingsBtn;
        private Label label1;
        private OpenFileDialog BrowseProgramDialog;
        private Components.ThemedToolTip HelpToolTip;
        private Label HelpLabel;
    }
}