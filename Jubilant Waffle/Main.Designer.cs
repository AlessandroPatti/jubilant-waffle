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
            this.StatusIcon = new System.Windows.Forms.PictureBox();
            this.AutoSaveIcon = new System.Windows.Forms.PictureBox();
            this.TransfersInLabel = new System.Windows.Forms.Label();
            this.TransfersOutLabel = new System.Windows.Forms.Label();
            this.SettingsIcon = new System.Windows.Forms.PictureBox();
            this.IconToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.DefaultFolderIcon = new System.Windows.Forms.PictureBox();
            this.ProgressBarsOutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.ProgressBarsInPanel = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.StatusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AutoSaveIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SettingsIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DefaultFolderIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // StatusIcon
            // 
            this.StatusIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.StatusIcon.BackColor = System.Drawing.Color.White;
            this.StatusIcon.ImageLocation = "icons\\status_off.png";
            this.StatusIcon.Location = new System.Drawing.Point(315, 0);
            this.StatusIcon.Name = "StatusIcon";
            this.StatusIcon.Size = new System.Drawing.Size(35, 35);
            this.StatusIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.StatusIcon.TabIndex = 0;
            this.StatusIcon.TabStop = false;
            this.IconToolTip.SetToolTip(this.StatusIcon, "Go online");
            // 
            // AutoSaveIcon
            // 
            this.AutoSaveIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AutoSaveIcon.BackColor = System.Drawing.Color.White;
            this.AutoSaveIcon.ImageLocation = "icons\\autosave_off.png";
            this.AutoSaveIcon.Location = new System.Drawing.Point(245, 0);
            this.AutoSaveIcon.Name = "AutoSaveIcon";
            this.AutoSaveIcon.Size = new System.Drawing.Size(35, 35);
            this.AutoSaveIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.AutoSaveIcon.TabIndex = 1;
            this.AutoSaveIcon.TabStop = false;
            this.IconToolTip.SetToolTip(this.AutoSaveIcon, "Automatically accept all incoming requests");
            // 
            // TransfersInLabel
            // 
            this.TransfersInLabel.AutoSize = true;
            this.TransfersInLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TransfersInLabel.Location = new System.Drawing.Point(2, 2);
            this.TransfersInLabel.Name = "TransfersInLabel";
            this.TransfersInLabel.Padding = new System.Windows.Forms.Padding(5);
            this.TransfersInLabel.Size = new System.Drawing.Size(109, 34);
            this.TransfersInLabel.TabIndex = 3;
            this.TransfersInLabel.Text = "Transfer In";
            this.TransfersInLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ShowTransferIn);
            // 
            // TransfersOutLabel
            // 
            this.TransfersOutLabel.AutoSize = true;
            this.TransfersOutLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TransfersOutLabel.Location = new System.Drawing.Point(107, 2);
            this.TransfersOutLabel.Margin = new System.Windows.Forms.Padding(0);
            this.TransfersOutLabel.Name = "TransfersOutLabel";
            this.TransfersOutLabel.Padding = new System.Windows.Forms.Padding(5);
            this.TransfersOutLabel.Size = new System.Drawing.Size(120, 34);
            this.TransfersOutLabel.TabIndex = 4;
            this.TransfersOutLabel.Text = "Transfer out";
            this.TransfersOutLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ShowTransferOut);
            // 
            // SettingsIcon
            // 
            this.SettingsIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SettingsIcon.BackColor = System.Drawing.Color.White;
            this.SettingsIcon.ErrorImage = null;
            this.SettingsIcon.ImageLocation = "icons\\settings.png";
            this.SettingsIcon.Location = new System.Drawing.Point(350, 0);
            this.SettingsIcon.Name = "SettingsIcon";
            this.SettingsIcon.Size = new System.Drawing.Size(35, 35);
            this.SettingsIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.SettingsIcon.TabIndex = 5;
            this.SettingsIcon.TabStop = false;
            this.IconToolTip.SetToolTip(this.SettingsIcon, "Access application settings");
            this.SettingsIcon.Click += new System.EventHandler(this.ChangeSettings);
            // 
            // DefaultFolderIcon
            // 
            this.DefaultFolderIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.DefaultFolderIcon.BackColor = System.Drawing.Color.White;
            this.DefaultFolderIcon.ImageLocation = "icons\\folder_default_off.png";
            this.DefaultFolderIcon.Location = new System.Drawing.Point(280, 0);
            this.DefaultFolderIcon.Name = "DefaultFolderIcon";
            this.DefaultFolderIcon.Size = new System.Drawing.Size(35, 35);
            this.DefaultFolderIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.DefaultFolderIcon.TabIndex = 8;
            this.DefaultFolderIcon.TabStop = false;
            this.IconToolTip.SetToolTip(this.DefaultFolderIcon, "Enable default folder");
            // 
            // ProgressBarsOutPanel
            // 
            this.ProgressBarsOutPanel.AutoScroll = true;
            this.ProgressBarsOutPanel.Location = new System.Drawing.Point(11, 45);
            this.ProgressBarsOutPanel.Name = "ProgressBarsOutPanel";
            this.ProgressBarsOutPanel.Size = new System.Drawing.Size(361, 304);
            this.ProgressBarsOutPanel.TabIndex = 9;
            this.ProgressBarsOutPanel.Visible = false;
            // 
            // ProgressBarsInPanel
            // 
            this.ProgressBarsInPanel.AutoScroll = true;
            this.ProgressBarsInPanel.Location = new System.Drawing.Point(11, 45);
            this.ProgressBarsInPanel.Name = "ProgressBarsInPanel";
            this.ProgressBarsInPanel.Size = new System.Drawing.Size(361, 304);
            this.ProgressBarsInPanel.TabIndex = 10;
            this.ProgressBarsInPanel.Visible = false;
            this.ProgressBarsInPanel.WrapContents = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.ControlBox = false;
            this.Controls.Add(this.ProgressBarsInPanel);
            this.Controls.Add(this.ProgressBarsOutPanel);
            this.Controls.Add(this.DefaultFolderIcon);
            this.Controls.Add(this.SettingsIcon);
            this.Controls.Add(this.TransfersOutLabel);
            this.Controls.Add(this.TransfersInLabel);
            this.Controls.Add(this.AutoSaveIcon);
            this.Controls.Add(this.StatusIcon);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Main";
            this.Opacity = 0.95D;
            this.ShowIcon = false;
            this.Text = "Main";
            this.Deactivate += new System.EventHandler(this.HideOnClickOut);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PreventClose);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            this.Leave += new System.EventHandler(this.HideOnClickOut);
            ((System.ComponentModel.ISupportInitialize)(this.StatusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AutoSaveIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SettingsIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DefaultFolderIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox StatusIcon;
        private System.Windows.Forms.PictureBox AutoSaveIcon;
        private System.Windows.Forms.Label TransfersInLabel;
        private System.Windows.Forms.Label TransfersOutLabel;
        private System.Windows.Forms.PictureBox SettingsIcon;
        private System.Windows.Forms.ToolTip IconToolTip;
        private System.Windows.Forms.PictureBox DefaultFolderIcon;
        public System.Windows.Forms.FlowLayoutPanel ProgressBarsOutPanel;
        public System.Windows.Forms.FlowLayoutPanel ProgressBarsInPanel;
    }
}