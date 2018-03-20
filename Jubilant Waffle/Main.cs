using System;
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
        private Graphics graphics;
        public Main() {
            InitializeComponent();
            
            #region Place box at bottom right
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width - 5, Screen.PrimaryScreen.WorkingArea.Height - this.Height - 5);
            #endregion
            #region Hide title bar, but keep borders
            FormBorderStyle = FormBorderStyle.FixedSingle;
            ControlBox = false;
            this.Text = "";
            #endregion
            #region Hide box when lose focus
            //TODO MouseLeave temporary, should be "on click out"
            //this.MouseLeave += (object s, EventArgs e) => this.Hide();
            #endregion
            #region Icons

            /* Create a hover effect that changes the background when mouse is hovering */
            DefaultFolderIcon.MouseEnter += (object s, EventArgs e) => DefaultFolderIcon.BackColor = Color.LightBlue;
            DefaultFolderIcon.MouseLeave += (object s, EventArgs e) => DefaultFolderIcon.BackColor = Color.White;
            StatusIcon.MouseEnter += (object s, EventArgs e) => StatusIcon.BackColor = Color.LightBlue;
            StatusIcon.MouseLeave += (object s, EventArgs e) => StatusIcon.BackColor = Color.White;
            AutoSaveIcon.MouseEnter += (object s, EventArgs e) => AutoSaveIcon.BackColor = Color.LightBlue;
            AutoSaveIcon.MouseLeave += (object s, EventArgs e) => AutoSaveIcon.BackColor = Color.White;
            SettingsIcon.MouseEnter += (object s, EventArgs e) => SettingsIcon.BackColor = Color.LightBlue;
            SettingsIcon.MouseLeave += (object s, EventArgs e) => SettingsIcon.BackColor = Color.White;
            TransfersOutLabel.MouseEnter += (object s, EventArgs e) => TransfersOutLabel.BackColor = Color.LightBlue;
            TransfersOutLabel.MouseLeave += (object s, EventArgs e) => TransfersOutLabel.BackColor = Color.White;
            TransfersInLabel.MouseEnter += (object s, EventArgs e) => TransfersInLabel.BackColor = Color.LightBlue;
            TransfersInLabel.MouseLeave += (object s, EventArgs e) => TransfersInLabel.BackColor = Color.White;
            
            /* Reacts to clicks */
            DefaultFolderIcon.MouseClick += ToggleDefaultFolder;
            StatusIcon.MouseClick += ToggleStatus;
            AutoSaveIcon.MouseClick += ToggleAutosave;
            #endregion
            graphics = this.CreateGraphics();

        }
        
        private void ToggleStatus(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Program.server.Status = !Program.server.Status;
                StatusIcon.ImageLocation = Program.server.Status ? @"icons\status_on.png" : @"icons\status_off.png";
                string tooltip = Program.server.Status ? "Go offline" : "Go online";
                this.IconToolTip.SetToolTip(this.StatusIcon, tooltip);
            }
        }
        private void ToggleDefaultFolder(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Program.server.UseDefault = !Program.server.UseDefault;
                DefaultFolderIcon.ImageLocation = Program.server.UseDefault ? @"icons\folder_default_on.png" : @"icons\folder_default_off.png";
                string tooltip = Program.server.UseDefault ? "Disable default folder" : "Enable default folder";
                this.IconToolTip.SetToolTip(this.DefaultFolderIcon, tooltip);
            }
        }
        private void ToggleAutosave(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Program.server.AutoSave = !Program.server.AutoSave;
                AutoSaveIcon.ImageLocation = Program.server.AutoSave ? @"icons\autosave_on.png" : @"icons\autosave_off.png";
                string tooltip = Program.server.AutoSave ? "Ask permission for each transfer" : "Automatically accept incoming requests";
                this.IconToolTip.SetToolTip(this.AutoSaveIcon, tooltip);
            }
        }
        private void ChangeSettings(object sender, EventArgs e) {
            Wizard w = new Wizard();
            w.ShowDialog();

        }

        private void PreventClose(object sender, FormClosingEventArgs e) {
            ///<summary>
            /// Added to the FormClosing event, will prevent the form to be closed in any way.
            ///</summary>
            e.Cancel = true;
        }
        private void HideOnClickOut(object sender, EventArgs e) {
            ProgressBarsInPanel.Hide();
            ProgressBarsOutPanel.Hide();
            this.Hide();
        }

        /* The main box contains two overlapped panels, one containing information 
         * on the outgoing transfer, the other for the incoming
         * The following two methods manage the visualization of one or the other
         */
        private void ShowTransferOut(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                #region Show Transfers
                ProgressBarsOutPanel.Show();
                ProgressBarsInPanel.Hide();
                this.RaisePaintEvent(this, null);
                #endregion
            }
        }
        private void ShowTransferIn(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                #region Show Transfers
                ProgressBarsInPanel.Show();
                ProgressBarsOutPanel.Hide();
                this.RaisePaintEvent(this, null);
                #endregion              
            }
        }

        private void CreateLineBelow(Control elem) {
            /// <summary>
            /// Draw a line under the control passed as argument. In the context of the application, 
            /// it is used to place a line that indicates which of the two panel is visible
            /// </summary>
            Pen myPen = new Pen(Color.CadetBlue, 5);
            int bottom = elem.Location.Y + elem.Height;
            int left = elem.Location.X + 15;
            int right = elem.Location.X + elem.Width - 15;

            /* It require the graphic to be redrawn */
            graphics.Clear(Color.White);
            CreateSeparator();
            graphics.DrawLine(myPen, left, bottom, right, bottom);
            myPen.Dispose();
        }

        private void OnPaint(object sender, System.Windows.Forms.PaintEventArgs e) {
            LoadIcons(null, null);
            CreateSeparator();
            if (ProgressBarsInPanel.Visible) {
                CreateLineBelow(TransfersInLabel);
            }
            else if (ProgressBarsOutPanel.Visible) {
                CreateLineBelow(TransfersOutLabel);
            }
        }

        private void LoadIcons(object sender, EventArgs e) {
            /// <summary>
            /// Update the icons with the current application settings
            /// </summary>
            string tooltip;
            AutoSaveIcon.ImageLocation = Program.server.AutoSave ? @"icons\autosave_on.png" : @"icons\autosave_off.png";
            tooltip = Program.server.AutoSave ? "Ask permission for each transfer" : "Automatically accept incoming requests";
            IconToolTip.SetToolTip(this.AutoSaveIcon, tooltip);

            DefaultFolderIcon.ImageLocation = Program.server.UseDefault ? @"icons\folder_default_on.png" : @"icons\folder_default_off.png";
            tooltip = Program.server.UseDefault ? "Disable default folder" : "Enable default folder";
            IconToolTip.SetToolTip(this.DefaultFolderIcon, tooltip);

            StatusIcon.ImageLocation = Program.server.Status ? @"icons\status_on.png" : @"icons\status_off.png";
            tooltip = Program.server.Status ? "Go offline" : "Go online";
            IconToolTip.SetToolTip(this.StatusIcon, tooltip);
        }
        private void CreateSeparator() {
            /// <summary>
            /// Draw a line that separates the buttons from the transfer panels.
            /// </summary>
            Pen myPen = new System.Drawing.Pen(System.Drawing.Color.LightBlue);
            graphics.DrawLine(myPen, 0, 37, 500, 37);
            myPen.Dispose();
        }
    }
}
