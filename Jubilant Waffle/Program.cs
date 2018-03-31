using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;

namespace Jubilant_Waffle {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static User self;                                    // Represent the user running the application
        public static Server server;                                // The server part of te application. It announces itself and waits for new connection from the client
        public static Client client;                                // The client part of the application. It collects announcements from other servers and sends files to them on user request.
        public static Main mainbox;                                 // The main box showed at the bottom right of the screen. It gives access to the option other then a view on pending and completed transfers.
        public static Wizard wizard;                                // Used to change main options of the application. It is automatically called at first launch of the application-
        public static Dictionary<String, User> users;               // The list of users that are online. Its collected by the client. The key will be the IP address, which has to be unique.

        public static NotifyIcon trayIcon;

        
        public static Mutex mutex = null;                           // Used to make the apllication single-instance.

        #region Program constant used by all modules
        public const string iconFile = "waffle_icon_3x_multiple.ico";              // The icon of the application
        public const string defaultImagePath = @"icons\default-user-image.png";    // The image file used when the connected user has not set any user pic.
        public const int port = 20000;                                             // The port used by the udp socket to listen for other user connection. The port "port+1" is used by the internal tcp socket instead.
        public const int timeout = 200000;                                         // The timeout used by the sockets on blocking opration. Determine the maximum ammount of time of wait.
        public const int bufferSize = 4 * 1024 * 1024;                             // The size of the buffer used to send file.
        public static string AppDataFolder;                                         // It will contains the path where data (user pic, settings file) will be stored. Typically %userdir%/AppData/Roaming/Jubilant Waffle

        /* The follong constants define the protocol used */
        public const byte FILE = 0;                                 // Used to request a new file transfer
        public const byte DIRECTORY = 1;                            // Used to request a new directory transfer
        public const byte HELLO = 2;                                // Used by the server to announce itself online
        public const byte BYE = 3;                                  // Used by the server when it switches to offline status
        public const byte INFORMATION_REQUEST = 4;                  // Used by the client when a new unknown user connects to ask for the name and eventually a pic
        public const byte INFO_WITH_IMAGE = 5;                      // Reply to a INFORMATION_REQUEST informing that also an image will be sent
        public const byte INFO_WITHOUT_IMAGE = 6;                   // Reply to a INFORMATION_REQUEST informing that also an image will be sent
        public const byte TRANSFER_OK = 7;                          // Positively reply to a transfer request
        public const byte TRANSFER_DENY = 8;                        // Negatively reply to a transfer request
        #endregion
        [STAThread]
        static void Main(string[] argv) {

            Application.SetCompatibleTextRenderingDefault(false);

            #region Make the application single instance and communitate with the main one
            /* 
             * We want the application to have only one instance. When another instance is opened 
             * it maight be one of the followng case:
             *      - The user tries to execute another instance by doubleclick: the application is immediately shutdown.
             *      - The user tries to send a file by right clicking on: this will launch a new instance that has to 
             *              communicate with the first one and send the path of the file.
             */

            bool createdNew;

            mutex = new System.Threading.Mutex(true, "Jubilant Waffle", out createdNew);        // Tries to instanciate a named mutex and acquire the onwneship. Created new will be true if ownership has been granted
            if (!createdNew) {
                if (argv.Length > 0) {
                    /* Im assuming that if the application has be launched
                     * with at least a paramenter, it's the right click case. 
                     */
                    string path = System.String.Join(" ", argv);                                // If the path of the right-clicked file contains spaces, the application will read then as several argments. 
                    if (File.Exists(path))                                                       // Just to be sure that it was a proper right-click launch.
                        Client.EnqueueMessage(path);                                            // Enstablish the communication with the main instance.
                }
                return;
            }
            #endregion
            #region Setup application
            AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Jubilant Waffle";
            users = new Dictionary<String, User>();
            self = new User("", GetMyIP());
            server = new Server();
            client = new Client();
            mainbox = new Main();
            #endregion
            #region Read settings or execute wizard
            if (!File.Exists(AppDataFolder + @"\settings.ini")) {
                /* The settings file is store each time the applicatation is closed.
                 * If the settings files is not present, I'm assiming that it's the first launch
                 * and thus the wizard is called for the first setup
                 */
                server.DefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Jubilant Waffle";
                wizard = new Wizard();
                
                /* The application will hang untill the wizard is over.
                 * The wizard can terminate with confirm or cancel. Only in the first case the application will continue the execution
                 * and settings file will be store to avoid displaying the setup again at startup.
                 */
                DialogResult res=wizard.ShowDialog();
                if (res == DialogResult.Cancel)
                    Environment.Exit(0);
                else
                    WriteSettingsFile();
            }
            else {
                ReadSettingsFile();
            }
            #endregion
            #region Tray Icon
            trayIcon = new NotifyIcon();
            trayIcon.Visible = true;

            /* Show a tooltip for current status when mouse is hovering */
            trayIcon.Text = "Jubilant Waffle\nStatus: ";
            trayIcon.Text += server.Status ? "Online" : "Offline";

            /* Set icon */
            trayIcon.Icon = new System.Drawing.Icon(iconFile);

            /* Show notification */
            trayIcon.ShowBalloonTip(500, "Jubilant Waffle", "Jubilant Waffle always runs minimized into tray", ToolTipIcon.None);
            trayIcon.BalloonTipClicked += (object s, EventArgs e) => mainbox.Show();            // When ballon tips are shown, I want the main box to be shown. Ballon tips will be shown for each new incoming file.

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

        private static void ReadSettingsFile() {
            string param, value;
            foreach (var line in File.ReadLines(AppDataFolder + @"\settings.ini")) {
                param = line.Substring(0, line.IndexOf(":"));
                value = line.Substring(line.IndexOf(":") + 1);
                switch (param) {
                    case "Autosave":
                        server.AutoSave = value == "True" ? true : false;
                        break;
                    case "UseDefault":
                        server.UseDefault = value == "True" ? true : false;
                        break;
                    case "DefaultPath":
                        server.DefaultPath = value;
                        break;
                    case "Status":
                        server.Status = value == "True" ? true : false;
                        break;
                    case "Pic":
                        self.imagePath = value == "Custom" ? AppDataFolder + @"\user.png" : defaultImagePath;
                        break;
                    case "PublicName":
                        self.publicName = value;
                        break;
                    case "Name":
                        self.name = value;
                        break;
                    case "Surname":
                        self.surname = value;
                        break;
                }
            }
        }
        private static void WriteSettingsFile() {
            System.IO.StreamWriter sw;
            if (!System.IO.Directory.Exists(AppDataFolder))
                System.IO.Directory.CreateDirectory(AppDataFolder);
            sw = new System.IO.StreamWriter(AppDataFolder + @"\settings.ini");
            sw.WriteLine("Autosave:" + (server.AutoSave ? "True" : "False"));
            sw.WriteLine("UseDefault:" + (server.UseDefault ? "True" : "False"));
            sw.WriteLine("DefaultPath:" + server.DefaultPath);
            sw.WriteLine("Status:" + (server.Status ? "True" : "False"));
            sw.WriteLine("Pic:" + (self.imagePath != null ? "Custom" : "Default"));
            sw.WriteLine("PublicName:" + self.publicName);
            sw.WriteLine("Name:" + self.name);
            sw.WriteLine("Surname:" + self.surname);
            sw.Close();
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
            WriteSettingsFile();
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


        /* The following methods are utilities to gather information abaout the network form the local machine */
        static public string GetMyIP() {
            /// <summary>
            /// Get the IP address of the first application with a valid IP address.
            /// Do not manage the cases where there are several interfaces connected.
            /// </summary>
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");

        }
        public static System.Net.IPAddress GetSubnetMask(System.Net.IPAddress address) {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces()) {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses) {
                    if (unicastIPAddressInformation.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                        if (address.Equals(unicastIPAddressInformation.Address)) {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }
        public static System.Net.IPAddress GetBroadcastAddress(this System.Net.IPAddress address, IPAddress subnetMask) {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++) {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new System.Net.IPAddress(broadcastAddress);
        }

    }
}
