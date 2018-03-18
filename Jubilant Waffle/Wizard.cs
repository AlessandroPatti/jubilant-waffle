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
    public partial class Wizard : Form {
        public Wizard() {
            InitializeComponent();
            this.FormClosing += PreventClose;

        }

        private void ConfirmW(object sender, EventArgs e) {
            System.IO.StreamWriter sw = new System.IO.StreamWriter("settings.ini");
            sw.WriteLine("Name:" + NameBox.Text);
            sw.WriteLine("Surname:" + SurnameBox.Text);
            sw.WriteLine("PublicName:" + PublicNameBox.Text);
            sw.WriteLine("DefaultPath:" + DefaultPathBox.Text);
            sw.WriteLine("AutoSave:" + (AutoSaveCheckbox.Checked ? "True" : "False"));
            sw.WriteLine("UseDefault:" + (UseDefaultCheckbox.Checked ? "True" : "False"));
            sw.WriteLine("Status:False");
            sw.Close();
            if ((new System.IO.FileInfo(UserPicBox.ImageLocation)).Extension != "png") {
                Image img = Image.FromFile(UserPicBox.ImageLocation);
                Bitmap bmp = new Bitmap(img);
                bmp.Save("user.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            else
                System.IO.File.Copy(UserPicBox.ImageLocation, "user.png");
            lock (Program.mutex) {
                Program.wizardRes = true;
                System.Threading.Monitor.PulseAll(Program.mutex);
            }
            this.FormClosing -= PreventClose;
            this.Close();
        }

        private void CancelW(object sender, EventArgs e) {
            lock (Program.mutex) {
                Program.wizardRes = false;
                System.Threading.Monitor.PulseAll(Program.mutex);
            }
            this.FormClosing -= PreventClose;
            this.Close();
        }

        private void PreventClose(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
        }

        private void UseDefaultCheckbox_CheckedChanged(object sender, EventArgs e) {
            if (UseDefaultCheckbox.Checked) {
                DefaultPathBox.ReadOnly = false;
                Browse.Enabled = true;
            }
            else {
                DefaultPathBox.ReadOnly = true;
                Browse.Enabled = false;
            }
        }

        private void BrowseFolders(object sender, EventArgs e) {
            DialogResult result = FolderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(FolderBrowserDialog.SelectedPath)) {
                DefaultPathBox.Text = FolderBrowserDialog.SelectedPath;
            }
        }

        private void ChangeUserPic(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                DialogResult result = FileDialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(FileDialog.FileName)) {
                    UserPicBox.ImageLocation = FileDialog.FileName;
                }
            }
        }
    }
}
