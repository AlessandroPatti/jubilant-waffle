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
            /// <summary>
            /// Load the application options from the memory and update the form elements according to them
            /// </summary>
            NameBox.Text = Program.self.name;
            SurnameBox.Text = Program.self.surname;
            PublicNameBox.Text = Program.self.publicName;
            AutoSaveCheckbox.Checked = Program.server.AutoSave;
            UseDefaultCheckbox.Checked = Program.server.UseDefault;
            DefaultPathBox.Text = Program.server.DefaultPath;
            UserPicBox.ImageLocation = Program.self.imagePath ?? @"icons\default-user-image.png";
        }
        private void WriteConfiguration() {
            /// <summary>
            /// Write the application options according to the value of the elements of the form
            /// This method is only called if the button confirm is clicked.
            /// </summary>
            /// 
            /// It does not store any information regarding the user pic, 
            /// which are instead managed by the method CorfirmW

            Program.self.name = NameBox.Text;
            Program.self.surname = SurnameBox.Text;
            Program.self.publicName = PublicNameBox.Text;
            Program.server.AutoSave = AutoSaveCheckbox.Checked;
            Program.server.UseDefault = UseDefaultCheckbox.Checked;
            Program.server.DefaultPath = DefaultPathBox.Text;
        }

        private void ConfirmW(object sender, EventArgs e) {
            /// <summary>
            /// Apply changes to options and close the form
            /// </summary>
            WriteConfiguration();
            /* The user pic is store in the %AppData% folder under the name "user.png" so that if the original file is deleted, the pic will not be lost.
             * Using this setup, the image has been changed since last time only if the UserPicBox.ImageLocation is either null or points to the "user.png" file
             */
            if (UserPicBox.ImageLocation != Program.AppDataFolder + @"\user.png" && UserPicBox.ImageLocation != null && UserPicBox.ImageLocation != @"icons\default-user-image.png") {
                if (!System.IO.Directory.Exists(Program.AppDataFolder)) {
                    System.IO.Directory.CreateDirectory(Program.AppDataFolder);
                }
                /* The image has tobe stored as png format. If it is not png, it will be loaded into a bitmap
                 * and converted to png (otherwise it's just copied)
                 */
                if ((new System.IO.FileInfo(UserPicBox.ImageLocation)).Extension != "png") {
                    Image img = Image.FromFile(UserPicBox.ImageLocation);
                    Bitmap bmp = new Bitmap(img);
                    if (System.IO.File.Exists(Program.AppDataFolder + @"\user.png")) {
                        System.IO.File.Delete(Program.AppDataFolder + @"\user.png");
                    }
                    bmp.Save(Program.AppDataFolder + @"\user.png", System.Drawing.Imaging.ImageFormat.Png);
                }
                else {
                    System.IO.File.Copy(UserPicBox.ImageLocation, Program.AppDataFolder + @"\user.png");
                }

                /* At the end, the user pic will be stored in the %AppData% folder */
                Program.self.imagePath = Program.AppDataFolder + @"\user.png";
            }
            this.FormClosing -= PreventClose;
            this.Close();
        }
        private void CancelW(object sender, EventArgs e) {
            /// <summary>
            /// Close the form without saving
            /// </summary>
            this.FormClosing -= PreventClose;
            this.Close();
        }

        private void UseDefaultCheckbox_CheckedChanged(object sender, EventArgs e) {
            /// <summary>
            /// Simple events that enable or disable the dialog to select the default path accourding to the value of UseDefault
            /// </summary>
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
            /// <summary>
            /// Allows the user to browse the FS through a dialog and eventually update the value of the default path
            /// </summary>
            DialogResult result = FolderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(FolderBrowserDialog.SelectedPath)) {
                DefaultPathBox.Text = FolderBrowserDialog.SelectedPath;
            }
        }
        private void ChangeUserPic(object sender, MouseEventArgs e) {
            /// <summary>
            /// Allows the user to browse the FS through a dialog and eventually update the user pic
            /// </summary>
            if (e.Button == MouseButtons.Left) {
                DialogResult result = FileDialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(FileDialog.FileName)) {
                    UserPicBox.ImageLocation = FileDialog.FileName;
                }
            }
        }

        private void PreventClose(object sender, FormClosingEventArgs e) {
            ///<summary>
            /// Added to the FormClosing event, will prevent the form to be closed in any way.
            ///</summary>
            e.Cancel = true;
        }

    }
}
