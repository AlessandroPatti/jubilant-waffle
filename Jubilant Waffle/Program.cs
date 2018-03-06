using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jubilant_Waffle {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>


        static private Server server;
        static private Client client;
        const string iconFile = "waffle_icon_3x_multiple.ico";
        static System.Windows.Forms.NotifyIcon trayIcon;

        static Main mainbox;
        private static System.Threading.Mutex mutex = null;
        [STAThread]
        static void Main() {

            #region
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            #endregion
            #region Server
            //TODO Name should be taken from a config file
            server = new Server(20000, "Alessandro");
            #endregion
            #region Client
            client = new Client();
            #endregion
            #region Make the application single instance
            // code at http://www.c-sharpcorner.com/UploadFile/f9f215/how-to-restrict-the-application-to-just-one-instance/
            bool createdNew;
            const string appName = "Jubilant Waffle";
            mutex = new System.Threading.Mutex(true, appName, out createdNew);
            if (!createdNew) {
                return;
            }
            #endregion
            #region main box
            mainbox = new Main(server, client);
            #endregion
            #region Tray Icon
            trayIcon = new NotifyIcon();
            trayIcon.Visible = true;

            /* Show a tooltip for current status */
            trayIcon.Text = "Jubilant Waffle\nStatus: ";
            trayIcon.Text += server.Status ? "Online" : "Offline";

            /* Set icon */
            trayIcon.Icon = new System.Drawing.Icon(iconFile);
            /* Execute Minimized and Hide Application (Only Tray) */
            //this.WindowState = FormWindowState.Minimized;
            //this.ShowInTaskbar = false;
            /* Show notification */
            trayIcon.ShowBalloonTip(500, "Jubilant Waffle", "Jubilant Waffle always runs minimized into tray", ToolTipIcon.None);
            trayIcon.BalloonTipClicked += (object s, EventArgs e) => mainbox.Show();
            /*
             * Set the mouse right click menu
             */
            MenuItem[] trayContextMenuItems = new MenuItem[2];
            /* Show Exit */
            trayContextMenuItems[0] = new MenuItem("Exit");
            trayContextMenuItems[0].Click += Exit;
            /* Show option to switch status. Text of the option change according to current status */
            trayContextMenuItems[1] = new MenuItem();
            trayContextMenuItems[1].Text = (server.Status) ? "Go offline" : "Go online";
            trayContextMenuItems[1].Click += ChangeStatus;

            trayIcon.ContextMenu = new ContextMenu(trayContextMenuItems);
            
            /* Show box on left click */
            trayIcon.MouseClick += ShowMainBox;
            #endregion
            #region Context Menu entry
            /*
             * Contex menu entries are stored under HKEY_CLASSES_ROOT for all users
             * and in HKEY_CURRENT_USER\Software\Classes for the current one.
             * Write an entry to HKEY_CLASSES_ROOT require high privileges, so
             * entry will be written in HKEY_CURRENT_USER.
             */
            /* Files */
            AddRegistryEntry(@"Software\Classes\*\shell\jubilant-waffle", "Share with Jubilant Waffle");
            AddRegistryEntry(@"Software\Classes\*\shell\jubilant-waffle\command", Application.ExecutablePath + " %1");
            AddRegistryEntry(@"Software\Classes\*\shell\jubilant-waffle\Icon", Application.ExecutablePath);
            /* Directory */
            AddRegistryEntry(@"Software\Classes\Directory\shell\jubilant-waffle", "Share with Jubilant Waffle");
            AddRegistryEntry(@"Software\Classes\Directory\shell\jubilant-waffle\command", Application.ExecutablePath + " %1");
            AddRegistryEntry(@"Software\Classes\Directory\shell\jubilant-waffle\Icon", Application.ExecutablePath);
            #endregion

            Application.Run();

        }

        private static void ShowMainBox(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left)
                mainbox.Show();
        }

        static private void ChangeStatus(object sender, EventArgs e) {
            server.Status = !server.Status;
            trayIcon.ContextMenu.MenuItems[1].Text = (server.Status) ? "Go offline" : "Go online";
            trayIcon.Text = "Jubilant Waffle\nStatus: ";
            trayIcon.Text += server.Status ? "Online" : "Offline";
        }

        static private void Exit(object sender, EventArgs e) {
            #region Delete Registry entry for contex menu
            /* Files */
            RemoveRegistryEntry(@"Software\Classes\*\shell\jubilant-waffle\command");
            RemoveRegistryEntry(@"Software\Classes\*\shell\jubilant-waffle\Icon");
            RemoveRegistryEntry(@"Software\Classes\*\shell\jubilant-waffle");
            /* Directory */
            RemoveRegistryEntry(@"Software\Classes\Directory\shell\jubilant-waffle\command");
            RemoveRegistryEntry(@"Software\Classes\Directory\shell\jubilant-waffle\Icon");
            RemoveRegistryEntry(@"Software\Classes\Directory\shell\jubilant-waffle");
            #endregion

            Application.Exit();
        }

        static private bool AddRegistryEntry(string key, string value) {
            Microsoft.Win32.RegistryKey reg = null;
            try {
                reg = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(key);
                if (reg != null)
                    reg.SetValue("", value);
            }
            catch (Exception) {
                //TODO manage error
                return false;
            }
            finally {
                if (reg != null)
                    reg.Close();
            }
            return true;
        }
        static private bool RemoveRegistryEntry(string key) {
            try {
                Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(key);
                if (reg != null) {
                    reg.Close();
                    Microsoft.Win32.Registry.CurrentUser.DeleteSubKey(key);
                }
            }
            catch (Exception) {
                //TODO manage error
                return false;
            }
            return true;
        }

    }
}
