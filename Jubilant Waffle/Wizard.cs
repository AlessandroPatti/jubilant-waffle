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
            NameBox.Text = Program.self.name;
            SurnameBox.Text = Program.self.surname;
            PublicNameBox.Text = Program.self.publicName;
            AutoSaveCheckbox.Checked = Program.server.AutoSave;
            UseDefaultCheckbox.Checked = Program.server.UseDefault;
            DefaultPathBox.Text = Program.server.DefaultPath;
            UserPicBox.ImageLocation = Program.self.imagePath;
        }

        private void WriteConfiguration() {
            Program.self.name = NameBox.Text;
            Program.self.surname = SurnameBox.Text;
            Program.self.publicName = PublicNameBox.Text;
            Program.server.AutoSave = AutoSaveCheckbox.Checked;
            Program.server.UseDefault = UseDefaultCheckbox.Checked;
            Program.server.DefaultPath = DefaultPathBox.Text;
        }

        private void ConfirmW(object sender, EventArgs e) {
            this.Hide();
            WriteConfiguration();
            if (UserPicBox.ImageLocation != "user.png" && UserPicBox.ImageLocation != "default-user-image.png")
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
            if (UserPicBox.ImageLocation != "user.png" && UserPicBox.ImageLocation != "default-user-image.png") {
                Program.self.imagePath = "user.png";
            }
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
