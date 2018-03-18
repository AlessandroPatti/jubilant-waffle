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
        public Dictionary<string, ProgressBar> ProgressBarsIn;
        public Dictionary<string, ProgressBar> ProgressBarsOut;
        //private Pen pen;
        private Graphics graphics;
        public Main() {
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
            #region Progress Bars
            ProgressBarsIn = new Dictionary<string, ProgressBar>();
            ProgressBarsOut = new Dictionary<string, ProgressBar>();
            #endregion
            #region Initiliaze graphics
            graphics = this.CreateGraphics();
            #endregion
        }

        delegate void AddProgressBarCallback(string name);
        public void AddProgressBarIn(string name) {
            if (this.InvokeRequired) {
                AddProgressBarCallback callback = new AddProgressBarCallback(AddProgressBarIn);
                this.Invoke(callback, name);
            }
            else {
                ProgressBar pbar = new ProgressBar();
                pbar.Name = name;
                int i = 0;
                while (ProgressBarsIn.ContainsKey(name + i.ToString())) {
                    i++;
                }
                ProgressBarsIn.Add(name + i.ToString(), pbar);
            }
        }
        public void AddProgressBarOut(string name) {
            if (this.InvokeRequired) {
                AddProgressBarCallback callback = new AddProgressBarCallback(AddProgressBarOut);
                this.Invoke(callback, name);
            }
            else {
                ProgressBar pbar = new ProgressBar();
                pbar.Name = name;
                /* What if two file with the same name are sent to the same user?
                 * 
                 int i = 0;
                while (ProgressBarsOut.ContainsKey(name + i.ToString())) {
                    i++;
                }
                ProgressBarsOut.Add(name + i.ToString(), pbar);
                */
                ProgressBarsOut.Add(name, pbar);
                //                TransferOutBox.BeginUpdate();
                TransferOutBox.Items.Add(pbar);
                TransferOutBox.Update();
            }
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

        private void PreventClose(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
        }

        private void HideOnClickOut(object sender, EventArgs e) {
            this.Hide();
        }

        private void ShowTransferOut(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                #region Show Transfers
                TransferInBox.Hide();
                TransferOutBox.Show();
                this.RaisePaintEvent(this, null);
                #endregion
            }
        }
        private void ShowTransferIn(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                #region Show Transfers
                TransferOutBox.Hide();
                TransferInBox.Show();
                this.RaisePaintEvent(this, null);
                #endregion              
            }
        }



        private void CreateSeparator() {
            System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.LightBlue);
            graphics.DrawLine(myPen, 0, 37, 500, 37);
            myPen.Dispose();
        }
        private void CreateLineBelow(Control elem) {
            System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.CadetBlue, 5);
            int bottom = elem.Location.Y + elem.Height;
            int left = elem.Location.X + 15;
            int right = elem.Location.X + elem.Width - 15;
            graphics.Clear(Color.White);
            CreateSeparator();
            graphics.DrawLine(myPen, left, bottom, right, bottom);
            myPen.Dispose();
        }
        private void OnPaint(object sender, System.Windows.Forms.PaintEventArgs e) {
            LoadIcons(null, null);
            CreateSeparator();
            if (TransferInBox.Visible) {
                CreateLineBelow(TransfersInLabel);
            }
            else {
                ;
            }
            if (TransferOutBox.Visible) {
                CreateLineBelow(TransfersOutLabel);
            }
            else {
                ;
            }
        }

        private void ChangeSettings(object sender, EventArgs e) {
            Wizard w = new Wizard();
            w.ShowDialog();

        }

        private void LoadIcons(object sender, EventArgs e) {
            string tooltip;
            AutoSaveIcon.ImageLocation = Program.server.AutoSave ? @"icons\autosave_on.png" : @"icons\autosave_off.png";
            tooltip = Program.server.AutoSave ? "Ask permission for each transfer" : "Automatically accept incoming requests";
            this.IconToolTip.SetToolTip(this.AutoSaveIcon, tooltip);
            DefaultFolderIcon.ImageLocation = Program.server.UseDefault ? @"icons\folder_default_on.png" : @"icons\folder_default_off.png";
            tooltip = Program.server.UseDefault ? "Disable default folder" : "Enable default folder";
            this.IconToolTip.SetToolTip(this.DefaultFolderIcon, tooltip);
            StatusIcon.ImageLocation = Program.server.Status ? @"icons\status_on.png" : @"icons\status_off.png";
            tooltip = Program.server.Status ? "Go offline" : "Go online";
            this.IconToolTip.SetToolTip(this.StatusIcon, tooltip);
        }
    }
}
