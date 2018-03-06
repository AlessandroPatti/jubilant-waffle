namespace Jubilant_Waffle {
    partial class Main {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.DefaultFolderIcon = new System.Windows.Forms.PictureBox();
            this.AutoSaveIcon = new System.Windows.Forms.PictureBox();
            this.ChangeFolderLabel = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.DefaultFolderIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AutoSaveIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // DefaultFolderIcon
            // 
            this.DefaultFolderIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.DefaultFolderIcon.BackColor = System.Drawing.Color.White;
            this.DefaultFolderIcon.ErrorImage = null;
            this.DefaultFolderIcon.ImageLocation = "folder_default_off.png";
            this.DefaultFolderIcon.Location = new System.Drawing.Point(334, 0);
            this.DefaultFolderIcon.Name = "DefaultFolderIcon";
            this.DefaultFolderIcon.Size = new System.Drawing.Size(50, 50);
            this.DefaultFolderIcon.TabIndex = 0;
            this.DefaultFolderIcon.TabStop = false;
            // 
            // AutoSaveIcon
            // 
            this.AutoSaveIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AutoSaveIcon.BackColor = System.Drawing.Color.White;
            this.AutoSaveIcon.ImageLocation = "autosave_off.png";
            this.AutoSaveIcon.Location = new System.Drawing.Point(287, 0);
            this.AutoSaveIcon.Name = "AutoSaveIcon";
            this.AutoSaveIcon.Size = new System.Drawing.Size(50, 50);
            this.AutoSaveIcon.TabIndex = 1;
            this.AutoSaveIcon.TabStop = false;
            // 
            // ChangeFolderLabel
            // 
            this.ChangeFolderLabel.ActiveLinkColor = System.Drawing.Color.White;
            this.ChangeFolderLabel.AutoSize = true;
            this.ChangeFolderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ChangeFolderLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(230)))));
            this.ChangeFolderLabel.Location = new System.Drawing.Point(234, 53);
            this.ChangeFolderLabel.Name = "ChangeFolderLabel";
            this.ChangeFolderLabel.Size = new System.Drawing.Size(150, 17);
            this.ChangeFolderLabel.TabIndex = 2;
            this.ChangeFolderLabel.TabStop = true;
            this.ChangeFolderLabel.Text = "Change Default Folder";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.ControlBox = false;
            this.Controls.Add(this.ChangeFolderLabel);
            this.Controls.Add(this.AutoSaveIcon);
            this.Controls.Add(this.DefaultFolderIcon);
            this.Name = "Main";
            this.Opacity = 0.95D;
            this.ShowIcon = false;
            this.Text = "Main";
            ((System.ComponentModel.ISupportInitialize)(this.DefaultFolderIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AutoSaveIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox DefaultFolderIcon;
        private System.Windows.Forms.PictureBox AutoSaveIcon;
        private System.Windows.Forms.LinkLabel ChangeFolderLabel;
    }
}