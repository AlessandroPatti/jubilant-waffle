namespace Jubilant_Waffle {
    partial class Client {
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
            this.UserListView = new System.Windows.Forms.ListView();
            this.Confirm = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // UserListView
            // 
            this.UserListView.Location = new System.Drawing.Point(12, 12);
            this.UserListView.Name = "UserListView";
            this.UserListView.Size = new System.Drawing.Size(760, 494);
            this.UserListView.TabIndex = 0;
            this.UserListView.UseCompatibleStateImageBehavior = false;
            // 
            // Confirm
            // 
            this.Confirm.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Confirm.Location = new System.Drawing.Point(640, 512);
            this.Confirm.Name = "Confirm";
            this.Confirm.Size = new System.Drawing.Size(121, 37);
            this.Confirm.TabIndex = 1;
            this.Confirm.Text = "Confirm";
            this.Confirm.UseVisualStyleBackColor = true;
            this.Confirm.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ConfirmSend);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(501, 512);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(121, 37);
            this.Cancel.TabIndex = 2;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.UndoSend);
            // 
            // Client
            // 
            this.AcceptButton = this.Cancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.Confirm;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.ControlBox = false;
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Confirm);
            this.Controls.Add(this.UserListView);
            this.Name = "Client";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView UserListView;
        private System.Windows.Forms.Button Confirm;
        private System.Windows.Forms.Button Cancel;
    }
}