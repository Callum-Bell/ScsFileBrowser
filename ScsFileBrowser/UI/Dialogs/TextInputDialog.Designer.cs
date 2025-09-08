namespace ScsFileBrowser.UI.Dialogs
{
    partial class TextInputDialog
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
            TextInputLabel = new Label();
            InputTextBox = new ScsFileBrowser.UI.Components.ThemedTextBox();
            ContinueBtn = new ScsFileBrowser.UI.Components.ThemedButton();
            SuspendLayout();
            // 
            // TextInputLabel
            // 
            TextInputLabel.AutoSize = true;
            TextInputLabel.Location = new Point(12, 9);
            TextInputLabel.Name = "TextInputLabel";
            TextInputLabel.Size = new Size(35, 15);
            TextInputLabel.TabIndex = 8;
            TextInputLabel.Text = "Input";
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
            InputTextBox.Size = new Size(246, 20);
            InputTextBox.TabIndex = 6;
            InputTextBox.TextChanged += InputTextBox_TextChanged;
            InputTextBox.KeyUp += InputTextBox_KeyUp;
            // 
            // ContinueBtn
            // 
            ContinueBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ContinueBtn.DialogResult = DialogResult.OK;
            ContinueBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(64, 61, 82);
            ContinueBtn.FlatStyle = FlatStyle.Flat;
            ContinueBtn.Location = new Point(12, 56);
            ContinueBtn.Name = "ContinueBtn";
            ContinueBtn.Size = new Size(246, 23);
            ContinueBtn.TabIndex = 5;
            ContinueBtn.Text = "Continue";
            ContinueBtn.UseVisualStyleBackColor = true;
            // 
            // TextInputDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(270, 91);
            Controls.Add(TextInputLabel);
            Controls.Add(InputTextBox);
            Controls.Add(ContinueBtn);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "TextInputDialog";
            Text = "TextInputDialog";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Label TextInputLabel;
        private Components.ThemedTextBox InputTextBox;
        private Components.ThemedButton ContinueBtn;
    }
}