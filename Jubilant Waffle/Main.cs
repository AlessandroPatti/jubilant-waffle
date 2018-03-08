﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jubilant_Waffle {
    public partial class Main : Form {
        Server server;
        Client client;
        public Main(Server server, Client client) {
            this.server = server;
            this.client = client;
            InitializeComponent();
            #region Place box at bottom right
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width - 5,
                                   Screen.PrimaryScreen.WorkingArea.Height - this.Height - 5);
            #endregion
            #region Hide title bar, but keep borders
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.ControlBox = false;
            this.Text = "";
            #endregion
            #region Hide box
            //TODO MouseLeave temporary, should be "on click out"
            //this.MouseLeave += (object s, EventArgs e) => this.Hide();
            #endregion
            #region Icons

            /* Create a hover effect that changes the background when mouse is over */
            DefaultFolderIcon.MouseEnter += (object s, EventArgs e) => DefaultFolderIcon.BackColor = Color.LightBlue;
            DefaultFolderIcon.MouseLeave += (object s, EventArgs e) => DefaultFolderIcon.BackColor = Color.White;
            AutoSaveIcon.MouseEnter += (object s, EventArgs e) => AutoSaveIcon.BackColor = Color.LightBlue;
            AutoSaveIcon.MouseLeave += (object s, EventArgs e) => AutoSaveIcon.BackColor = Color.White;
            SettingsIcon.MouseEnter += (object s, EventArgs e) => SettingsIcon.BackColor = Color.LightBlue;
            SettingsIcon.MouseLeave += (object s, EventArgs e) => SettingsIcon.BackColor = Color.White;
            /* Reacts to clicks */
            DefaultFolderIcon.MouseClick += ToggleDefaultFolder;
            AutoSaveIcon.MouseClick += ToggleAutosave;
            #endregion
            
        }
        
        private void ToggleDefaultFolder(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                server._useDefault = !server._useDefault;
                DefaultFolderIcon.ImageLocation = server._useDefault ? "folder_default_on.png" : "folder_default_off.png";
                string tooltip = server._useDefault ? "Disable default folder" : "Enable default folder";
                this.IconToolTip.SetToolTip(this.DefaultFolderIcon, tooltip);
            }
        }

        private void ToggleAutosave(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                server._autoSave = !server._autoSave;
                AutoSaveIcon.ImageLocation = server._autoSave ? "autosave_on.png" : "autosave_off.png";
                string tooltip = server._autoSave ? "Ask permission for each transfer" : "Automatically accept incoming requests";
                this.IconToolTip.SetToolTip(this.AutoSaveIcon, tooltip);
            }
        }

        private void Main_Paint(object sender, PaintEventArgs e) {
            #region create separator
            System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.LightBlue);
            System.Drawing.Graphics formGraphics = this.CreateGraphics();
            formGraphics.DrawLine(myPen, 0, 37, 500, 37);
            myPen.Dispose();
            formGraphics.Dispose();
            #endregion
        }

        private void ShowProfile(object sender, EventArgs e) {

        }
    }
}