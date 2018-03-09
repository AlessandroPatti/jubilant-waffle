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

        public void AddProgressBarIn(string name) {
            ProgressBar pbar = new ProgressBar();
            pbar.Name = name;
            int i = 0;
            while (ProgressBarsIn.ContainsKey(name + i.ToString())) {
                i++;
            }
            ProgressBarsIn.Add(name + i.ToString(), pbar);
        }
        public void AddProgressBarOut(string name) {
            ProgressBar pbar = new ProgressBar();
            pbar.Name = name;
            int i = 0;
            while (ProgressBarsOut.ContainsKey(name + i.ToString())) {
                i++;
            }
            ProgressBarsOut.Add(name + i.ToString(), pbar);
        }

        private void ToggleDefaultFolder(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Program.server._useDefault = !Program.server._useDefault;
                DefaultFolderIcon.ImageLocation = Program.server._useDefault ? "folder_default_on.png" : "folder_default_off.png";
                string tooltip = Program.server._useDefault ? "Disable default folder" : "Enable default folder";
                this.IconToolTip.SetToolTip(this.DefaultFolderIcon, tooltip);
            }
        }

        private void ToggleAutosave(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Program.server._autoSave = !Program.server._autoSave;
                AutoSaveIcon.ImageLocation = Program.server._autoSave ? "autosave_on.png" : "autosave_off.png";
                string tooltip = Program.server._autoSave ? "Ask permission for each transfer" : "Automatically accept incoming requests";
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
            System.Diagnostics.Debug.WriteLine("Paint");
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
    }
}
