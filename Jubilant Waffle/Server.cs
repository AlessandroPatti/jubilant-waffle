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
    public partial class Server : Form {

        #region UDPClient
        /* The UDP client is only in charge of sending periodically an announcement if the the status is online */
        System.Net.Sockets.UdpClient udp; //The client
        System.Timers.Timer announceTimer; //The timer that will execute periodically the announcement
        #endregion
        #region TCPSever
        System.Net.Sockets.TcpListener tcp;
        #endregion

        private string _defaultPath = "";
        private bool _useDefault = false;
        private bool _autoSave = false;
        private bool _cancelCurrent = false; // This is used to undo the current transfer
        private bool _status = false; //The status true means online.

        const int port = 20000;
        const int timeout = 200000;
        const int timer = 2000;

        public bool UseDefault {
            get {
                return _useDefault;
            }
            set {
                _useDefault = value;
            }
        }
        public string DefaultPath {
            get {
                return _defaultPath;
            }
            set {
                _defaultPath = value;
            }
        }
        public bool AutoSave {
            get {
                return _autoSave;
            }
            set {
                _autoSave = value;
            }
        }
        public bool Status {
            get {
                return _status;
            }
            set {
                /* If the user set true and server is not running, start both services */
                if (value && !_status) {
                    tcp.Start();
                    System.Threading.Thread t = new System.Threading.Thread(() => ServerRoutine());
                    t.Name = "Server Routine";
                    t.Start();
                    announceTimer.Enabled = true;
                }
                /* If the user set false and server is running, stop both services */
                if (!value && _status) {
                    tcp.Stop();
                    announceTimer.Enabled = false;
                }
                _status = value;
            }
        }
        public Server() {
            InitializeComponent();
            #region UDP Client/Server and Timer setup
            udp = new System.Net.Sockets.UdpClient();
            System.Net.IPAddress ip, mask, broadcast;
            ip = System.Net.IPAddress.Parse(Program.self.ip);
            mask = Program.GetSubnetMask(ip);
            broadcast = Program.GetBroadcastAddress(ip, mask);
            udp.Connect(broadcast, port); //Set the default IP address. It is the broadcast address of the subnet
            udp.EnableBroadcast = true;
            /* 
             * Setup the timer for the announcement
             */
            announceTimer = new System.Timers.Timer();
            announceTimer.Elapsed += Announce; // The callback that will execute the announcement has to be added to the event Elapsed
            announceTimer.Interval = timer; // The timeout interval
            announceTimer.AutoReset = true; // Make the execution repeat several times
            #endregion 
            #region TCP Server setup
            tcp = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, port);
            tcp.Server.ReceiveTimeout = tcp.Server.SendTimeout = timeout; // The timeout set the maxiumum amount of time that the Listener will wait befor throwing and exception
            #endregion
        }

        private void Announce(object sender, System.Timers.ElapsedEventArgs e) {
            ///This delegate is called to send the announcement in broadcast at each timer elapse. 
            ///The string sent is "HELLO"
            byte[] announce = System.Text.Encoding.ASCII.GetBytes("HELLO"); //TODO Eventually replace the string with a constant
            udp.Send(announce, announce.Length); //TODO Send or Send Async?
        }

        private void ServerRoutine() {
            ///Put the server in listen mode. Each new connection is executed into a new separate thread.
            System.Net.Sockets.TcpClient incomingConnection;
            while (_status) {
                #region Wait for new connections
                try {
                    incomingConnection = tcp.AcceptTcpClient();
                }
                catch (System.Net.Sockets.SocketException) {
                    /* 
                     * SocketException is launched at timeout elased. Nothing as to be done, 
                     * but thanks to the timeout the status condition is periodically checked.
                     */
                    continue;
                }
                #endregion
                #region new incoming connection
                (new System.Threading.Thread(() => ManageConnection(incomingConnection))).Start();
                #endregion
            }
        }

        private void ManageConnection(System.Net.Sockets.TcpClient client) {
            /// Manage new incoming connection. Connection can be of two types
            ///     - Request for file transfer, control message 'FILE!'
            ///     - Rest for user info, control message 'WHO??'

            byte[] data = new byte[5];
            string msg;
            #region Read request
            try {
                client.GetStream().Read(data, 0, 5);
            }
            catch (System.Net.Sockets.SocketException) {
                /* Could not connect to the host, something went wrong. Request aborted */
                System.Console.Write("Impossible parse request, control message not received");
                return;
            }
            msg = System.Text.Encoding.ASCII.GetString(data);
            #endregion
            #region Parse request
            switch (msg) {
                case "FILE!":
                    ReceiveFile(client);
                    break;
                case "WHO??":
                    SendPersonalInfo(client);
                    break;
            }
            #endregion
        }

        private void SendPersonalInfo(System.Net.Sockets.TcpClient client) {
            byte[] data;
            long imageLenght;
            #region Send response
            if (Program.self.imagePath != null)
                data = System.Text.Encoding.ASCII.GetBytes("MARIO");
            else
                data = System.Text.Encoding.ASCII.GetBytes("LUIGI");
            try {
                client.GetStream().Write(data, 0, data.Length);
            }
            catch (System.Net.Sockets.SocketException) {
                /* Could not connect to the host, something went wrong. Request aborted */
                System.Console.Write("Impossible serving personal info request, failed sending response");
                return;
            }
            #endregion
            #region Send name lenght
            data = System.BitConverter.GetBytes(Program.self.name.Length);
            try {
                client.GetStream().Write(data, 0, data.Length);
            }
            catch (System.Net.Sockets.SocketException) {
                /* Could not connect to the host, something went wrong. Request aborted */
                System.Console.Write("Impossible serving personal info request, failed sending name lenght");
                return;
            }
            #endregion
            #region Send name
            data = System.Text.Encoding.ASCII.GetBytes(Program.self.name);
            try {
                client.GetStream().Write(data, 0, data.Length);
            }
            catch (System.Net.Sockets.SocketException) {
                /* Could not connect to the host, something went wrong. Request aborted */
                System.Console.Write("Impossible serving personal info request, failed sending name");
                return;
            }
            #endregion
            if (Program.self.imagePath != null) {
                #region Send image lenght
                imageLenght = (new System.IO.FileInfo(Program.self.imagePath)).Length;
                data = System.BitConverter.GetBytes(imageLenght);
                try {
                    client.GetStream().Write(data, 0, data.Length);
                }
                catch (System.Net.Sockets.SocketException) {
                    /* Could not connect to the host, something went wrong. Request aborted */
                    System.Console.Write("Impossible serving personal info request, failed sending image lenght");
                    return;
                }
                #endregion
                #region Send Image
                System.IO.FileStream fs;
                /* Open the file */
                try {
                    fs = System.IO.File.OpenRead(Program.self.imagePath);
                }
                catch (System.IO.FileNotFoundException) {
                    /* Could not find the file. File will not be sent */
                    System.Console.Write("Impossible send file, file not found");
                    return;
                }
                /* Send the file */
                data = new byte[4 * 1024 * 1024];
                long dataSent = 0;
                long fileSize = (new System.IO.FileInfo(Program.self.imagePath)).Length;
                while (dataSent < fileSize) {
                    int sizeOfLastRead = 0;
                    try {
                        sizeOfLastRead = fs.Read(data, 0, (int)System.Math.Min(fileSize - dataSent, (long)data.Length));
                    }
                    catch {
                        /* Could not read the file. File will not be sent */
                        System.Console.Write("Impossible send file, error will reading");
                        return;
                    }
                    client.GetStream().Write(data, 0, sizeOfLastRead);
                    dataSent += sizeOfLastRead;
                }

                #endregion
            }
        }

        private void ReceiveFile(System.Net.Sockets.TcpClient client) {
            byte[] data;
            int fileNameLenght;
            string filename, path;
            long fileSize;
            long alreadyReceived = 0;
            int sizeOfLastRead;
            System.IO.FileStream fs;
            #region Read file name lenght
            data = new byte[4];
            try {
                client.GetStream().Read(data, 0, 4);
            }
            catch (System.Net.Sockets.SocketException) {
                /* Could not connect to the host, something went wrong. Request aborted */
                System.Console.Write("Impossible receiving file, failed reading file name lenght");
                return;
            }
            fileNameLenght = System.BitConverter.ToInt32(data, 0);
            #endregion
            #region Read file name
            data = new byte[fileNameLenght];
            try {
                client.GetStream().Read(data, 0, fileNameLenght);
            }
            catch (System.Net.Sockets.SocketException) {
                /* Could not connect to the host, something went wrong. Request aborted */
                System.Console.Write("Impossible receiving file, failed reading file name");
                return;
            }
            filename = System.Text.Encoding.ASCII.GetString(data);
            #endregion
            #region Response
            if (!AutoSave) {
                //TODO Prompt accept connection
                string name;
                lock (Program.users) {
                    try {
                        name = Program.users[((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()].name;
                    }
                    catch (KeyNotFoundException e) {
                        //TODO maybe ask for name?
                        name = "unknown";
                    }
                }
                DialogResult res = MessageBox.Show("User '" + name + "' is willing to send you the file '" + filename + "'. Do you want to accept?", "", MessageBoxButtons.YesNo);
                if (res == DialogResult.No) {
                    // User refused the file
                    return;
                }
            }
            if (!UseDefault) {
                DialogResult res = FolderSelectionDialog.ShowDialog();
                if (res == DialogResult.OK) {
                    path = FolderSelectionDialog.SelectedPath + @"\" + filename;
                }
                else {
                    return;
                }
            }
            else {
                path = DefaultPath + @"\" + filename;
            }
            #endregion
            #region Read file size
            data = new byte[8];
            try {
                client.GetStream().Read(data, 0, 8);
            }
            catch (System.Net.Sockets.SocketException) {
                /* Could not connect to the host, something went wrong. Request aborted */
                System.Console.Write("Impossible receiving file, failed reading file size");
                return;
            }
            fileSize = System.BitConverter.ToInt64(data, 0);
            #endregion
            #region Receive file
            data = new byte[4 * 1024 * 1024];
            //TODO existing file will be automatically overwritten. Modify this behaviour later
            fs = new System.IO.FileStream(path, System.IO.FileMode.Create);
            while (alreadyReceived < fileSize && !_cancelCurrent) {
                try {
                    sizeOfLastRead = client.GetStream().Read(data, 0, (int)System.Math.Min((long)4 * 1024 * 1024, fileSize - alreadyReceived));
                }
                catch (System.Net.Sockets.SocketException) {
                    /* Could not connect to the host, something went wrong. Request aborted */
                    System.Console.Write("Impossible receiving file, failed reading file size");
                    return;
                }
                alreadyReceived += sizeOfLastRead;
                System.Diagnostics.Debug.WriteLine("Sent " + alreadyReceived.ToString() + "B out of " + fileSize.ToString() + "B");
                fs.Write(data, 0, sizeOfLastRead);
            }
            /* reset cancelCurrent. It assures that if it has been sent, it wont be active for next file in the list */
            _cancelCurrent = false;
            fs.Close();
            #endregion
            #region Unzip
            //TODO unzip file
            #endregion
        }
    }
}
