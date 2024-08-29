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
            this.TextInputLabel = new System.Windows.Forms.Label();
            this.InputTextBox = new ScsFileBrowser.UI.Components.ThemedTextBox();
            this.CancelBtn = new ScsFileBrowser.UI.Components.ThemedButton();
            this.ContinueBtn = new ScsFileBrowser.UI.Components.ThemedButton();
            this.SuspendLayout();
            // 
            // TextInputLabel
            // 
            this.TextInputLabel.AutoSize = true;
            this.TextInputLabel.Location = new System.Drawing.Point(12, 9);
            this.TextInputLabel.Name = "TextInputLabel";
            this.TextInputLabel.Size = new System.Drawing.Size(35, 15);
            this.TextInputLabel.TabIndex = 8;
            this.TextInputLabel.Text = "Input";
            // 
            // InputTextBox
            // 
            this.InputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(35)))), ((int)(((byte)(58)))));
            this.InputTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.InputTextBox.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.InputTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(222)))), ((int)(((byte)(244)))));
            this.InputTextBox.Location = new System.Drawing.Point(12, 27);
            this.InputTextBox.Name = "InputTextBox";
            this.InputTextBox.Size = new System.Drawing.Size(246, 20);
            this.InputTextBox.TabIndex = 6;
            this.InputTextBox.TextChanged += new System.EventHandler(this.InputTextBox_TextChanged);
            this.InputTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.InputTextBox_KeyUp);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(61)))), ((int)(((byte)(82)))));
            this.CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelBtn.Location = new System.Drawing.Point(138, 56);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(120, 23);
            this.CancelBtn.TabIndex = 4;
            this.CancelBtn.Text = "Success";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // ContinueBtn
            // 
            this.ContinueBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ContinueBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ContinueBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(61)))), ((int)(((byte)(82)))));
            this.ContinueBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ContinueBtn.Location = new System.Drawing.Point(12, 56);
            this.ContinueBtn.Name = "ContinueBtn";
            this.ContinueBtn.Size = new System.Drawing.Size(120, 23);
            this.ContinueBtn.TabIndex = 5;
            this.ContinueBtn.Text = "Continue";
            this.ContinueBtn.UseVisualStyleBackColor = true;
            // 
            // TextInputDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 91);
            this.Controls.Add(this.TextInputLabel);
            this.Controls.Add(this.InputTextBox);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.ContinueBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "TextInputDialog";
            this.Text = "TextInputDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label TextInputLabel;
        private Components.ThemedTextBox InputTextBox;
        private Components.ThemedButton CancelBtn;
        private Components.ThemedButton ContinueBtn;
    }
}