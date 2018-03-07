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
            this.components = new System.ComponentModel.Container();
            this.DefaultFolderIcon = new System.Windows.Forms.PictureBox();
            this.AutoSaveIcon = new System.Windows.Forms.PictureBox();
            this.IncomingLabel = new System.Windows.Forms.Label();
            this.TransfersLabel = new System.Windows.Forms.Label();
            this.SettingsIcon = new System.Windows.Forms.PictureBox();
            this.IconToolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.DefaultFolderIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AutoSaveIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SettingsIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // DefaultFolderIcon
            // 
            this.DefaultFolderIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.DefaultFolderIcon.BackColor = System.Drawing.Color.White;
            this.DefaultFolderIcon.ErrorImage = null;
            this.DefaultFolderIcon.ImageLocation = "folder_default_off.png";
            this.DefaultFolderIcon.Location = new System.Drawing.Point(315, 0);
            this.DefaultFolderIcon.Name = "DefaultFolderIcon";
            this.DefaultFolderIcon.Size = new System.Drawing.Size(35, 35);
            this.DefaultFolderIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.DefaultFolderIcon.TabIndex = 0;
            this.DefaultFolderIcon.TabStop = false;
            this.IconToolTip.SetToolTip(this.DefaultFolderIcon, "Enable default folder");
            // 
            // AutoSaveIcon
            // 
            this.AutoSaveIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AutoSaveIcon.BackColor = System.Drawing.Color.White;
            this.AutoSaveIcon.ImageLocation = "autosave_off.png";
            this.AutoSaveIcon.Location = new System.Drawing.Point(280, 0);
            this.AutoSaveIcon.Name = "AutoSaveIcon";
            this.AutoSaveIcon.Size = new System.Drawing.Size(35, 35);
            this.AutoSaveIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.AutoSaveIcon.TabIndex = 1;
            this.AutoSaveIcon.TabStop = false;
            this.IconToolTip.SetToolTip(this.AutoSaveIcon, "Automatically accept all incoming requests");
            // 
            // IncomingLabel
            // 
            this.IncomingLabel.AutoSize = true;
            this.IncomingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IncomingLabel.Location = new System.Drawing.Point(7, 7);
            this.IncomingLabel.Name = "IncomingLabel";
            this.IncomingLabel.Size = new System.Drawing.Size(99, 24);
            this.IncomingLabel.TabIndex = 3;
            this.IncomingLabel.Text = "Transfer In";
            this.IncomingLabel.Click += new System.EventHandler(this.ShowProfile);
            // 
            // TransfersLabel
            // 
            this.TransfersLabel.AutoSize = true;
            this.TransfersLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TransfersLabel.Location = new System.Drawing.Point(112, 7);
            this.TransfersLabel.Name = "TransfersLabel";
            this.TransfersLabel.Size = new System.Drawing.Size(110, 24);
            this.TransfersLabel.TabIndex = 4;
            this.TransfersLabel.Text = "Transfer out";
            // 
            // SettingsIcon
            // 
            this.SettingsIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SettingsIcon.BackColor = System.Drawing.Color.White;
            this.SettingsIcon.ErrorImage = null;
            this.SettingsIcon.ImageLocation = "settings.png";
            this.SettingsIcon.Location = new System.Drawing.Point(350, 0);
            this.SettingsIcon.Name = "SettingsIcon";
            this.SettingsIcon.Size = new System.Drawing.Size(35, 35);
            this.SettingsIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.SettingsIcon.TabIndex = 5;
            this.SettingsIcon.TabStop = false;
            this.IconToolTip.SetToolTip(this.SettingsIcon, "Access application settings");
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.ControlBox = false;
            this.Controls.Add(this.SettingsIcon);
            this.Controls.Add(this.TransfersLabel);
            this.Controls.Add(this.IncomingLabel);
            this.Controls.Add(this.AutoSaveIcon);
            this.Controls.Add(this.DefaultFolderIcon);
            this.Name = "Main";
            this.Opacity = 0.95D;
            this.ShowIcon = false;
            this.Text = "Main";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Main_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.DefaultFolderIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AutoSaveIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SettingsIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox DefaultFolderIcon;
        private System.Windows.Forms.PictureBox AutoSaveIcon;
        private System.Windows.Forms.Label IncomingLabel;
        private System.Windows.Forms.Label TransfersLabel;
        private System.Windows.Forms.PictureBox SettingsIcon;
        private System.Windows.Forms.ToolTip IconToolTip;
    }
}