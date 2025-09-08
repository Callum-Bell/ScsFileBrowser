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
            ContinueBtn = new ScsFileBrowser.UI.Components.ThemedButton();
            InputTextBox = new ScsFileBrowser.UI.Components.ThemedTextBox();
            TextInputLabel = new Label();
            ComboBoxInputLabel = new Label();
            InputComboBox = new ComboBox();
            SuspendLayout();
            // 
            // ContinueBtn
            // 
            ContinueBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ContinueBtn.DialogResult = DialogResult.OK;
            ContinueBtn.FlatStyle = FlatStyle.Flat;
            ContinueBtn.Location = new Point(12, 111);
            ContinueBtn.Name = "ContinueBtn";
            ContinueBtn.Size = new Size(253, 23);
            ContinueBtn.TabIndex = 0;
            ContinueBtn.Text = "Continue";
            ContinueBtn.UseVisualStyleBackColor = true;
            // 
            // InputTextBox
            // 
            InputTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            InputTextBox.BackColor = Color.FromArgb(38, 35, 58);
            InputTextBox.BorderStyle = BorderStyle.None;
            InputTextBox.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            InputTextBox.ForeColor = Color.FromArgb(224, 222, 244);
            InputTextBox.Location = new Point(12, 27);
            InputTextBox.Name = "InputTextBox";
            InputTextBox.Size = new Size(253, 20);
            InputTextBox.TabIndex = 1;
            InputTextBox.TextChanged += InputTextBox_TextChanged;
            // 
            // TextInputLabel
            // 
            TextInputLabel.AutoSize = true;
            TextInputLabel.Location = new Point(12, 9);
            TextInputLabel.Name = "TextInputLabel";
            TextInputLabel.Size = new Size(35, 15);
            TextInputLabel.TabIndex = 2;
            TextInputLabel.Text = "Input";
            // 
            // ComboBoxInputLabel
            // 
            ComboBoxInputLabel.AutoSize = true;
            ComboBoxInputLabel.Location = new Point(12, 50);
            ComboBoxInputLabel.Name = "ComboBoxInputLabel";
            ComboBoxInputLabel.Size = new Size(35, 15);
            ComboBoxInputLabel.TabIndex = 2;
            ComboBoxInputLabel.Text = "Input";
            // 
            // InputComboBox
            // 
            InputComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            InputComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            InputComboBox.FlatStyle = FlatStyle.Flat;
            InputComboBox.FormattingEnabled = true;
            InputComboBox.Location = new Point(12, 68);
            InputComboBox.Name = "InputComboBox";
            InputComboBox.Size = new Size(253, 23);
            InputComboBox.TabIndex = 3;
            InputComboBox.SelectedIndexChanged += InputComboBox_SelectedIndexChanged;
            // 
            // ComboTextInputDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(277, 146);
            Controls.Add(InputComboBox);
            Controls.Add(ComboBoxInputLabel);
            Controls.Add(TextInputLabel);
            Controls.Add(InputTextBox);
            Controls.Add(ContinueBtn);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "ComboTextInputDialog";
            Text = "ComboInputDialog";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Components.ThemedButton ContinueBtn;
        private Components.ThemedTextBox InputTextBox;
        private Label TextInputLabel;
        private Label ComboBoxInputLabel;
        private ComboBox InputComboBox;
    }
}