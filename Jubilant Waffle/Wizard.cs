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

        private void LoadConfiguration(object sender, EventArgs e) {
            if (System.IO.File.Exists("settings.ini")) {
                string param, value;
                foreach (var line in System.IO.File.ReadLines("settings.ini")) {
                    param = line.Substring(0, line.IndexOf(":"));
                    value = line.Substring(line.IndexOf(":") + 1);
                    switch (param) {
                        case "Name":
                            NameBox.Text = value;
                            break;
                        case "Surname":
                            SurnameBox.Text = value;
                            break;
                        case "Autosave":
                            AutoSaveCheckbox.Checked = value == "True" ? true : false;
                            break;
                        case "UseDefault":
                            UseDefaultCheckbox.Checked = value == "True" ? true : false;
                            break;
                        case "DefaultPath":
                            DefaultPathBox.Text = value;
                            break;
                        case "Pic":
                            UserPicBox.ImageLocation = value == "Custom" && System.IO.File.Exists("user.png") ? "user.png" : "default-user-image.png";
                            break;
                        case "PublicName":
                            PublicNameBox.Text = value;
                            break;
                    }
                }
            }
        }

        private void ConfirmW(object sender, EventArgs e) {
            this.Hide();
            System.IO.StreamWriter sw = new System.IO.StreamWriter("settings.ini");
            sw.WriteLine("Name:" + NameBox.Text);
            sw.WriteLine("Surname:" + SurnameBox.Text);
            sw.WriteLine("PublicName:" + PublicNameBox.Text);
            sw.WriteLine("DefaultPath:" + DefaultPathBox.Text);
            sw.WriteLine("AutoSave:" + (AutoSaveCheckbox.Checked ? "True" : "False"));
            sw.WriteLine("UseDefault:" + (UseDefaultCheckbox.Checked ? "True" : "False"));
            sw.WriteLine("Status:False");
            sw.WriteLine("Pic:" + (UserPicBox.ImageLocation == "default-user-image.png" ? "Default" : "Custom"));
            sw.Close();
            if(UserPicBox.ImageLocation != "user.png" && UserPicBox.ImageLocation != "default-user-image.png")
            if ((new System.IO.FileInfo(UserPicBox.ImageLocation)).Extension != "png") {
                Image img = Image.FromFile(UserPicBox.ImageLocation);
                Bitmap bmp = new Bitmap(img);
                if (System.IO.File.Exists("user.png")) {
                    System.IO.File.Delete("user.png");
                }
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
