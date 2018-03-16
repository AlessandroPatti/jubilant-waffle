﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jubilant_Waffle {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static public User self; // Represent the user running the application
        static public Server server;
        static public Client client;
        static public Main mainbox;
        static public System.Collections.Generic.Dictionary<String, User> users;

        const string iconFile = "waffle_icon_3x_multiple.ico";
        static System.Windows.Forms.NotifyIcon trayIcon;

        private static System.Threading.Mutex mutex = null;
        [STAThread]
        static void Main(string[] argv) {

            #region
            //If the following line is enable ImageList is not going to be populated
            //Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            #endregion
            #region Make the application single instance
            // code at http://www.c-sharpcorner.com/UploadFile/f9f215/how-to-restrict-the-application-to-just-one-instance/
            bool createdNew;
            const string appName = "Jubilant Waffle";
            mutex = new System.Threading.Mutex(true, appName, out createdNew);
            if (!createdNew) {
                if(argv.Length > 0) {
                    string path = System.String.Join(" ", argv);
                    Client.EnqueueMessage(path);
                }
                return;
            }
            #endregion
            #region Setup application
            //TODO Name should be taken from a config file
            users = new System.Collections.Generic.Dictionary<string, User>();
            self = new User("Alessandro", GetMyIP(), null);
            server = new Server();
            client = new Client();
            #endregion
            #region main box
            mainbox = new Main();
            #endregion
            #region Tray Icon
            trayIcon = new NotifyIcon();
            trayIcon.Visible = true;

            /* Show a tooltip for current status */
            trayIcon.Text = "Jubilant Waffle\nStatus: ";
            trayIcon.Text += server.Status ? "Online" : "Offline";

            /* Set icon */
            trayIcon.Icon = new System.Drawing.Icon(iconFile);

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
            if (e.Button == MouseButtons.Left) {
                mainbox.Show();
                mainbox.Focus();
            }
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
            trayIcon.Visible = false;
            Environment.Exit(0);
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



        static private string GetMyIP() {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");

        }
    }
}
