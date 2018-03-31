namespace Jubilant_Waffle {
    partial class Wizard {
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
            this.UserPicBox = new System.Windows.Forms.PictureBox();
            this.NameLabel = new System.Windows.Forms.Label();
            this.NameBox = new System.Windows.Forms.TextBox();
            this.SurnameBox = new System.Windows.Forms.TextBox();
            this.SurnameLabel = new System.Windows.Forms.Label();
            this.PublicNameBox = new System.Windows.Forms.TextBox();
            this.PublicNameLabel = new System.Windows.Forms.Label();
            this.CameraPicBox = new System.Windows.Forms.PictureBox();
            this.AutoSaveCheckbox = new System.Windows.Forms.CheckBox();
            this.UseDefaultCheckbox = new System.Windows.Forms.CheckBox();
            this.DefaultPathBox = new System.Windows.Forms.TextBox();
            this.Browse = new System.Windows.Forms.Button();
            this.Confirm = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.FileDialog = new System.Windows.Forms.OpenFileDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.ErrorPublicName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.UserPicBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CameraPicBox)).BeginInit();
            this.SuspendLayout();
            // 
            // UserPicBox
            // 
            this.UserPicBox.ImageLocation = "icons\\default-user-image.png";
            this.UserPicBox.Location = new System.Drawing.Point(25, 25);
            this.UserPicBox.Name = "UserPicBox";
            this.UserPicBox.Size = new System.Drawing.Size(150, 150);
            this.UserPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.UserPicBox.TabIndex = 0;
            this.UserPicBox.TabStop = false;
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.NameLabel.Location = new System.Drawing.Point(205, 23);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(51, 20);
            this.NameLabel.TabIndex = 1;
            this.NameLabel.Text = "Name";
            // 
            // NameBox
            // 
            this.NameBox.Location = new System.Drawing.Point(207, 46);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(297, 20);
            this.NameBox.TabIndex = 2;
            // 
            // SurnameBox
            // 
            this.SurnameBox.Location = new System.Drawing.Point(207, 101);
            this.SurnameBox.Name = "SurnameBox";
            this.SurnameBox.Size = new System.Drawing.Size(297, 20);
            this.SurnameBox.TabIndex = 4;
            // 
            // SurnameLabel
            // 
            this.SurnameLabel.AutoSize = true;
            this.SurnameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.SurnameLabel.Location = new System.Drawing.Point(206, 78);
            this.SurnameLabel.Name = "SurnameLabel";
            this.SurnameLabel.Size = new System.Drawing.Size(74, 20);
            this.SurnameLabel.TabIndex = 3;
            this.SurnameLabel.Text = "Surname";
            // 
            // PublicNameBox
            // 
            this.PublicNameBox.Location = new System.Drawing.Point(209, 155);
            this.PublicNameBox.Name = "PublicNameBox";
            this.PublicNameBox.Size = new System.Drawing.Size(295, 20);
            this.PublicNameBox.TabIndex = 6;
            // 
            // PublicNameLabel
            // 
            this.PublicNameLabel.AutoSize = true;
            this.PublicNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.PublicNameLabel.Location = new System.Drawing.Point(206, 132);
            this.PublicNameLabel.Name = "PublicNameLabel";
            this.PublicNameLabel.Size = new System.Drawing.Size(101, 20);
            this.PublicNameLabel.TabIndex = 5;
            this.PublicNameLabel.Text = "Public name*";
            // 
            // CameraPicBox
            // 
            this.CameraPicBox.BackColor = System.Drawing.Color.Transparent;
            this.CameraPicBox.ImageLocation = "icons\\edit.png";
            this.CameraPicBox.Location = new System.Drawing.Point(25, 145);
            this.CameraPicBox.Name = "CameraPicBox";
            this.CameraPicBox.Size = new System.Drawing.Size(30, 30);
            this.CameraPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CameraPicBox.TabIndex = 7;
            this.CameraPicBox.TabStop = false;
            this.CameraPicBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChangeUserPic);
            // 
            // AutoSaveCheckbox
            // 
            this.AutoSaveCheckbox.AutoSize = true;
            this.AutoSaveCheckbox.Checked = true;
            this.AutoSaveCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AutoSaveCheckbox.Location = new System.Drawing.Point(53, 223);
            this.AutoSaveCheckbox.Name = "AutoSaveCheckbox";
            this.AutoSaveCheckbox.Size = new System.Drawing.Size(203, 17);
            this.AutoSaveCheckbox.TabIndex = 11;
            this.AutoSaveCheckbox.Text = "Automatically accept all incoming files";
            this.AutoSaveCheckbox.UseVisualStyleBackColor = true;
            // 
            // UseDefaultCheckbox
            // 
            this.UseDefaultCheckbox.AutoSize = true;
            this.UseDefaultCheckbox.Location = new System.Drawing.Point(53, 246);
            this.UseDefaultCheckbox.Name = "UseDefaultCheckbox";
            this.UseDefaultCheckbox.Size = new System.Drawing.Size(178, 17);
            this.UseDefaultCheckbox.TabIndex = 12;
            this.UseDefaultCheckbox.Text = "Save all files in the default folder";
            this.UseDefaultCheckbox.UseVisualStyleBackColor = true;
            this.UseDefaultCheckbox.CheckedChanged += new System.EventHandler(this.UseDefaultCheckbox_CheckedChanged);
            // 
            // DefaultPathBox
            // 
            this.DefaultPathBox.Location = new System.Drawing.Point(70, 270);
            this.DefaultPathBox.Name = "DefaultPathBox";
            this.DefaultPathBox.ReadOnly = true;
            this.DefaultPathBox.Size = new System.Drawing.Size(353, 20);
            this.DefaultPathBox.TabIndex = 13;
            // 
            // Browse
            // 
            this.Browse.Enabled = false;
            this.Browse.Location = new System.Drawing.Point(429, 268);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(75, 23);
            this.Browse.TabIndex = 14;
            this.Browse.Text = "Browse";
            this.Browse.UseVisualStyleBackColor = true;
            this.Browse.Click += new System.EventHandler(this.BrowseFolders);
            // 
            // Confirm
            // 
            this.Confirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Confirm.Location = new System.Drawing.Point(275, 335);
            this.Confirm.Name = "Confirm";
            this.Confirm.Size = new System.Drawing.Size(90, 30);
            this.Confirm.TabIndex = 15;
            this.Confirm.Text = "Confim";
            this.Confirm.UseVisualStyleBackColor = true;
            this.Confirm.Click += new System.EventHandler(this.ConfirmW);
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(175, 335);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(90, 30);
            this.Cancel.TabIndex = 16;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.CancelW);
            // 
            // FileDialog
            // 
            this.FileDialog.FileName = "FileDialog";
            // 
            // ErrorPublicName
            // 
            this.ErrorPublicName.AutoSize = true;
            this.ErrorPublicName.ForeColor = System.Drawing.Color.Red;
            this.ErrorPublicName.Location = new System.Drawing.Point(313, 137);
            this.ErrorPublicName.Name = "ErrorPublicName";
            this.ErrorPublicName.Size = new System.Drawing.Size(90, 13);
            this.ErrorPublicName.TabIndex = 17;
            this.ErrorPublicName.Text = "Cannot be empty!";
            this.ErrorPublicName.Visible = false;
            // 
            // Wizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(524, 375);
            this.Controls.Add(this.ErrorPublicName);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Confirm);
            this.Controls.Add(this.Browse);
            this.Controls.Add(this.DefaultPathBox);
            this.Controls.Add(this.UseDefaultCheckbox);
            this.Controls.Add(this.AutoSaveCheckbox);
            this.Controls.Add(this.CameraPicBox);
            this.Controls.Add(this.PublicNameBox);
            this.Controls.Add(this.PublicNameLabel);
            this.Controls.Add(this.SurnameBox);
            this.Controls.Add(this.SurnameLabel);
            this.Controls.Add(this.NameBox);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.UserPicBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Wizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wizard";
            this.Load += new System.EventHandler(this.LoadConfiguration);
            ((System.ComponentModel.ISupportInitialize)(this.UserPicBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CameraPicBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox UserPicBox;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.TextBox SurnameBox;
        private System.Windows.Forms.Label SurnameLabel;
        private System.Windows.Forms.TextBox PublicNameBox;
        private System.Windows.Forms.Label PublicNameLabel;
        private System.Windows.Forms.PictureBox CameraPicBox;
        private System.Windows.Forms.CheckBox AutoSaveCheckbox;
        private System.Windows.Forms.CheckBox UseDefaultCheckbox;
        private System.Windows.Forms.TextBox DefaultPathBox;
        private System.Windows.Forms.Button Browse;
        private System.Windows.Forms.Button Confirm;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog;
        private System.Windows.Forms.OpenFileDialog FileDialog;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label ErrorPublicName;
    }
}