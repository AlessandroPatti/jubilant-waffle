namespace Jubilant_Waffle {
    partial class Settings {
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
            this.DefaultFolderLabel = new System.Windows.Forms.Label();
            this.DefaultFolderTextBox = new System.Windows.Forms.TextBox();
            this.DefaultFolderCheckbox = new System.Windows.Forms.CheckBox();
            this.AutoAcceptLabel = new System.Windows.Forms.Label();
            this.AutoAcceptRadioOn = new System.Windows.Forms.RadioButton();
            this.AutoAcceptRadioOff = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // DefaultFolderLabel
            // 
            this.DefaultFolderLabel.AutoSize = true;
            this.DefaultFolderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DefaultFolderLabel.Location = new System.Drawing.Point(103, 9);
            this.DefaultFolderLabel.Name = "DefaultFolderLabel";
            this.DefaultFolderLabel.Size = new System.Drawing.Size(308, 20);
            this.DefaultFolderLabel.TabIndex = 0;
            this.DefaultFolderLabel.Text = "Where do you want to save incoming files?";
            // 
            // DefaultFolderTextBox
            // 
            this.DefaultFolderTextBox.Location = new System.Drawing.Point(107, 32);
            this.DefaultFolderTextBox.Name = "DefaultFolderTextBox";
            this.DefaultFolderTextBox.Size = new System.Drawing.Size(321, 20);
            this.DefaultFolderTextBox.TabIndex = 1;
            // 
            // DefaultFolderCheckbox
            // 
            this.DefaultFolderCheckbox.AutoSize = true;
            this.DefaultFolderCheckbox.Location = new System.Drawing.Point(434, 34);
            this.DefaultFolderCheckbox.Name = "DefaultFolderCheckbox";
            this.DefaultFolderCheckbox.Size = new System.Drawing.Size(79, 17);
            this.DefaultFolderCheckbox.TabIndex = 2;
            this.DefaultFolderCheckbox.Text = "Always ask";
            this.DefaultFolderCheckbox.UseVisualStyleBackColor = true;
            // 
            // AutoAcceptLabel
            // 
            this.AutoAcceptLabel.AutoSize = true;
            this.AutoAcceptLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.AutoAcceptLabel.Location = new System.Drawing.Point(103, 89);
            this.AutoAcceptLabel.Name = "AutoAcceptLabel";
            this.AutoAcceptLabel.Size = new System.Drawing.Size(390, 20);
            this.AutoAcceptLabel.TabIndex = 4;
            this.AutoAcceptLabel.Text = "Do you want to automatically accept all incoming files?";
            // 
            // AutoAcceptRadioOn
            // 
            this.AutoAcceptRadioOn.AutoSize = true;
            this.AutoAcceptRadioOn.Location = new System.Drawing.Point(141, 122);
            this.AutoAcceptRadioOn.Name = "AutoAcceptRadioOn";
            this.AutoAcceptRadioOn.Size = new System.Drawing.Size(94, 17);
            this.AutoAcceptRadioOn.TabIndex = 6;
            this.AutoAcceptRadioOn.TabStop = true;
            this.AutoAcceptRadioOn.Text = "Always accept";
            this.AutoAcceptRadioOn.UseVisualStyleBackColor = true;
            // 
            // AutoAcceptRadioOff
            // 
            this.AutoAcceptRadioOff.AutoSize = true;
            this.AutoAcceptRadioOff.Location = new System.Drawing.Point(296, 122);
            this.AutoAcceptRadioOff.Name = "AutoAcceptRadioOff";
            this.AutoAcceptRadioOff.Size = new System.Drawing.Size(101, 17);
            this.AutoAcceptRadioOff.TabIndex = 7;
            this.AutoAcceptRadioOff.TabStop = true;
            this.AutoAcceptRadioOff.Text = "Ask for each file";
            this.AutoAcceptRadioOff.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 403);
            this.Controls.Add(this.AutoAcceptRadioOff);
            this.Controls.Add(this.AutoAcceptRadioOn);
            this.Controls.Add(this.AutoAcceptLabel);
            this.Controls.Add(this.DefaultFolderCheckbox);
            this.Controls.Add(this.DefaultFolderTextBox);
            this.Controls.Add(this.DefaultFolderLabel);
            this.Name = "Settings";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label DefaultFolderLabel;
        private System.Windows.Forms.TextBox DefaultFolderTextBox;
        private System.Windows.Forms.CheckBox DefaultFolderCheckbox;
        private System.Windows.Forms.Label AutoAcceptLabel;
        private System.Windows.Forms.RadioButton AutoAcceptRadioOn;
        private System.Windows.Forms.RadioButton AutoAcceptRadioOff;
    }
}