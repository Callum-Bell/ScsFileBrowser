namespace ScsFileBrowser.UI.Dialogs
{
    sealed partial class ComboTextInputDialog
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
            this.ContinueBtn = new ScsFileBrowser.UI.Components.ThemedButton();
            this.CancelBtn = new ScsFileBrowser.UI.Components.ThemedButton();
            this.InputTextBox = new ScsFileBrowser.UI.Components.ThemedTextBox();
            this.TextInputLabel = new System.Windows.Forms.Label();
            this.ComboBoxInputLabel = new System.Windows.Forms.Label();
            this.InputComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // ContinueBtn
            // 
            this.ContinueBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ContinueBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ContinueBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ContinueBtn.Location = new System.Drawing.Point(12, 111);
            this.ContinueBtn.Name = "ContinueBtn";
            this.ContinueBtn.Size = new System.Drawing.Size(120, 23);
            this.ContinueBtn.TabIndex = 0;
            this.ContinueBtn.Text = "Continue";
            this.ContinueBtn.UseVisualStyleBackColor = true;
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelBtn.Location = new System.Drawing.Point(145, 111);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(120, 23);
            this.CancelBtn.TabIndex = 0;
            this.CancelBtn.Text = "Success";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // InputTextBox
            // 
            this.InputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.InputTextBox.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.InputTextBox.Location = new System.Drawing.Point(12, 27);
            this.InputTextBox.Name = "InputTextBox";
            this.InputTextBox.Size = new System.Drawing.Size(253, 20);
            this.InputTextBox.TabIndex = 1;
            this.InputTextBox.TextChanged += new System.EventHandler(this.InputTextBox_TextChanged);
            // 
            // TextInputLabel
            // 
            this.TextInputLabel.AutoSize = true;
            this.TextInputLabel.Location = new System.Drawing.Point(12, 9);
            this.TextInputLabel.Name = "TextInputLabel";
            this.TextInputLabel.Size = new System.Drawing.Size(35, 15);
            this.TextInputLabel.TabIndex = 2;
            this.TextInputLabel.Text = "Input";
            // 
            // ComboBoxInputLabel
            // 
            this.ComboBoxInputLabel.AutoSize = true;
            this.ComboBoxInputLabel.Location = new System.Drawing.Point(12, 50);
            this.ComboBoxInputLabel.Name = "ComboBoxInputLabel";
            this.ComboBoxInputLabel.Size = new System.Drawing.Size(35, 15);
            this.ComboBoxInputLabel.TabIndex = 2;
            this.ComboBoxInputLabel.Text = "Input";
            // 
            // InputComboBox
            // 
            this.InputComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.InputComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.InputComboBox.FormattingEnabled = true;
            this.InputComboBox.Location = new System.Drawing.Point(12, 68);
            this.InputComboBox.Name = "InputComboBox";
            this.InputComboBox.Size = new System.Drawing.Size(253, 23);
            this.InputComboBox.TabIndex = 3;
            this.InputComboBox.SelectedIndexChanged += new System.EventHandler(this.InputComboBox_SelectedIndexChanged);
            // 
            // ComboTextInputDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 146);
            this.Controls.Add(this.InputComboBox);
            this.Controls.Add(this.ComboBoxInputLabel);
            this.Controls.Add(this.TextInputLabel);
            this.Controls.Add(this.InputTextBox);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.ContinueBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ComboTextInputDialog";
            this.Text = "ComboInputDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Components.ThemedButton ContinueBtn;
        private Components.ThemedButton CancelBtn;
        private Components.ThemedTextBox InputTextBox;
        private Label TextInputLabel;
        private Label ComboBoxInputLabel;
        private ComboBox InputComboBox;
    }
}