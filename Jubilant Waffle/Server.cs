﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Jubilant_Waffle {
    public class Server {

        #region Sockets and services
        UdpClient udp;                              // The client used to anounce the server on the net
        System.Timers.Timer announceTimer;          // The timer that will execute periodically the announcement
        TcpListener tcp;                            // Tcp server listening for users that want to start a transfer
        #endregion

        FolderBrowserDialog FolderSelectionDialog;
        DialogResult res;
        private object fsLock, optionLock;

        private const int timer = 2000;              // The frequency with which the server will announce itselft on the net is the status is online

        #region Application Options
        private string _defaultPath = "";
        private bool _useDefault = true;
        private bool _autoSave = true;
        private bool _status = false;

        public bool UseDefault {
            get {
                lock (optionLock) {
                    return _useDefault;
                }
            }
            set {
                lock (optionLock) {
                    _useDefault = value;
                }
            }
        }                   // Used when the user wants to automatically store all the file in a default folder
        public string DefaultPath {
            get {
                lock (optionLock) {
                    return _defaultPath;
                }
            }
            set {
                lock (optionLock) {
                    _defaultPath = value;
                }
            }
        }                // Define whether the user wants to use the default path or be asked at each transfer
        public bool AutoSave {
            get {
                lock (optionLock) {
                    return _autoSave;
                }
            }
            set {
                lock (optionLock) {
                    _autoSave = value;
                }
            }
        }                     // Define whether incoming file transfer will be automatically accepted or not
        public bool Status {
            get {
                lock (optionLock) {
                    return _status;
                }
            }
            set {
                lock (optionLock) {
                    if (value && !_status) {
                        /* If the user set true and server is not running, start both services */
                        tcp.Start();
                        Thread t = new Thread(() => ServerRoutine());
#if DEBUG
                        t.Name = "Server Routine";
#endif
                        t.Start();
                        announceTimer.Enabled = true;
                    }
                    if (!value && _status) {
                        /* If the user set false and server is running, stop both services */
                        tcp.Stop();
                        announceTimer.Enabled = false;
                        byte[] dgram = { Program.BYE };
                        udp.Send(dgram, dgram.Length);
                    }
                    _status = value;
                }
            }
        }                       // The server will announce itself on the net only if the status is online (i.e. true)
        #endregion

        public Server() {
            fsLock = new object();
            optionLock = new object();
            FolderSelectionDialog = new FolderBrowserDialog();
            #region UDP Client/Server and Timer setup
            udp = new UdpClient();
            IPAddress ip, mask, broadcast;
            ip = IPAddress.Parse(Program.self.ip);
            mask = Program.GetSubnetMask(ip);
            broadcast = Program.GetBroadcastAddress(ip, mask);
            udp.Connect(broadcast, Program.port);                   // Set the default IP address. It is the broadcast address of the subnet
            /* Setup the timer for the announcement */
            announceTimer = new System.Timers.Timer();
            announceTimer.Elapsed += Announce;
            announceTimer.Interval = timer;
            announceTimer.AutoReset = true;                         // Make the execution repeat several times instead of once
            #endregion 
            #region TCP Server setup
            tcp = new TcpListener(IPAddress.Any, Program.port);
            tcp.Server.ReceiveTimeout = tcp.Server.SendTimeout = Program.timeout;
            #endregion
        }

        private void Announce(object sender, System.Timers.ElapsedEventArgs e) {
            /// <summary>
            /// Called by the timer to periodically announce the server
            ///</summary>
            byte[] dgram = { Program.HELLO };
            udp.Send(dgram, dgram.Length);
        }

        private void ServerRoutine() {
            /// <summary>
            /// Listen for new connection and execute a thread for each of them
            /// </summary>

            TcpClient client;
            while (Status) {
                #region Wait for new connections
                try {
                    client = tcp.AcceptTcpClient();
                }
                catch (SocketException) {
                    /* 
                     * SocketException is launched at timeout elased. Nothing as to be done, 
                     * but thanks to the timeout the status condition is periodically checked.
                     */
                    continue;
                }
                #endregion
                (new Thread(() => ManageConnection(client))).Start();
            }
        }
        private void ManageConnection(TcpClient client) {
            /// <summary>
            /// Parse the request by reading the control message received (i.e. FILE, DIRECTORY or INFORMATION_REQUEST)
            /// and execute the correct thread
            /// </summary>
            client.Client.ReceiveTimeout = client.Client.SendTimeout = Program.timeout;
            byte[] msg = new byte[1];
            #region Read request
            try {
                client.GetStream().Read(msg, 0, msg.Length);
            }
            catch (Exception e){
                if (e is SocketException || e is IOException) {
                    Debug.Write("Impossible parse request, control message not received");
                    return;
                }
                else {
                    throw e;
                }
            }
            #endregion
            #region Parse request
            try {
                switch (msg[0]) {
                    case Program.FILE:
                    case Program.DIRECTORY:
                        ReceiveFile(client, msg[0]);
                        break;
                    case Program.INFORMATION_REQUEST:
                        SendPersonalInfo(client);
                        break;
                }
            }
            catch (IOException) {

                return;
            }
            #endregion
        }

        private void SendPersonalInfo(TcpClient client) {
            /// <summary>
            /// Send user personal information (i.e. public name, user pic) to the remote party
            /// 
            /// This method may throw SocketException if the socket timeout expires.
            /// </summary>
            byte[] data;                       // Buffer for socket
            long fileSize;                     // Support variable that will contain the size of the user pic file if it is present
            FileStream fs = null;
            #region Send response
            /* If an image has been set by the user, it will be sent (INFO_WITH_IMAGE) otherwise only the name will be sent (INFO_WITHOUT_IMAGE)
             * The application will also check that the image file is still present before sending the control message.
             */
            if (Program.self.imagePath != null) {
                /* The user has set an image */
                try {
                    /* Instead of just checking the existence of the file, it is opened and kept open until the method finishes.
                     * This avoids that the file might be somehow deleted during the method execution (the file can't be deleted if in use)
                     */
                    fs = File.OpenRead(Program.self.imagePath);
                    data = new byte[] { Program.INFO_WITH_IMAGE };
                }
                catch (FileNotFoundException) {
                    /* The file is not present, so it will not be sent */
                    Program.self.imagePath = null;
                    data = new byte[] { Program.INFO_WITHOUT_IMAGE };
                }
            }
            else {
                data = new byte[] { Program.INFO_WITHOUT_IMAGE };
            }

            client.GetStream().Write(data, 0, data.Length);
            #endregion
            #region Send name lenght
            data = BitConverter.GetBytes(Program.self.publicName.Length);
            client.GetStream().Write(data, 0, data.Length);
            #endregion
            #region Send name
            data = Encoding.ASCII.GetBytes(Program.self.publicName);
            client.GetStream().Write(data, 0, data.Length);
            #endregion
            #region Send image if available
            if (Program.self.imagePath != null) {
                #region Send image lenght
                fileSize = (new FileInfo(Program.self.imagePath)).Length;
                data = BitConverter.GetBytes(fileSize);
                client.GetStream().Write(data, 0, data.Length);
                #endregion
                #region Send Image
                data = new byte[Program.bufferSize];
                long dataSent = 0;
                while (dataSent < fileSize) {
                    int sizeOfLastRead = 0;
                    try {
                        sizeOfLastRead = fs.Read(data, 0, (int)Math.Min(fileSize - dataSent, (long)data.Length));
                    }
                    catch {
                        Debug.Write("Impossible send file, error will reading");
                        return;
                    }
                    client.GetStream().Write(data, 0, sizeOfLastRead);
                    dataSent += sizeOfLastRead;
                }

                #endregion
            }
            #endregion
        }
        private void ReceiveFile(TcpClient client, byte type) {
            /// <summary>
            /// Enstablish a connect with the remote party and manage the reception of a file.
            /// Several dialog may be launched according to the application settings (i.e. AutoSave, UseDefault)
            /// 
            /// The method may lauch SocketException if the socket time out expires
            /// </summary>
            byte[] data;                // Buffer for the socket
            int lenght;                 // Support variable that will contains the lenght of the next message in the stream (i.e. filename)
            string filename, path;
            long fileSize;
            int sizeOfLastRead;
            FileStream fs;
            #region Read file name lenght
            data = new byte[4];
            client.GetStream().Read(data, 0, 4);
            lenght = BitConverter.ToInt32(data, 0);
            #endregion
            #region Read file name
            data = new byte[lenght];
            client.GetStream().Read(data, 0, lenght);
            filename = Encoding.ASCII.GetString(data);
            #endregion
            #region Response
            if (!AutoSave) {
                /* The user wants to be notified each time a user tries to send a file */
                string name;
                lock (Program.users) {
                    try {
                        name = Program.users[((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()].publicName;
                    }
                    catch (KeyNotFoundException e) {
                        //TODO maybe ask for name?
                        name = "unknown";
                    }
                }
                string msg = "User '" + name + "' is willing to send you the ";
                msg += type == Program.FILE ? ("file '" + filename) : ("folder '" + Path.GetFileNameWithoutExtension(filename));
                msg += "'. Do you want to accept?";
                DialogResult res = MessageBox.Show(msg, "", MessageBoxButtons.YesNo);
                if (res == DialogResult.No) {
                    client.GetStream().WriteByte(Program.TRANSFER_DENY);
                    // User refused the file
                    return;
                }
            }
            client.GetStream().WriteByte(Program.TRANSFER_OK);
            if (!UseDefault) {
                /* FolderSelectionDialog requires STA thread, but this thread is MTA.
                 * A new thread STA is launched to open the dialog
                 */
                Thread t = new Thread(() => res = FolderSelectionDialog.ShowDialog());
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                t.Join();
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
            client.GetStream().Read(data, 0, 8);
            fileSize = BitConverter.ToInt64(data, 0);
            #endregion
            #region Handle filename conflicts
            /* If the file already exists, the current file will be stored under the name <filename>(1).*
             * If <filename>(1).* already exists, it will be stored as <filename>(2).* and so on
             */
            lock (fsLock) {
                /* The lock is acquire to possible race conditions in those cases when two request are served
                 * at the same time for file that have the same name.
                 * 
                 * WARNING: it does not consider cases in which the another file is being created by another application 
                 * or by the user!
                 */
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(path))) {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                }
                if (File.Exists(path)) {
                    string noEx = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path);     // Full path with no file extension
                    int i = 1;                                                     // File number
                    string ex = Path.GetExtension(path);          // The extension of the file
                    while (File.Exists(path)) {
                        path = noEx + "(" + i.ToString() + ")" + ex;
                        i++;
                    }
                }

                /* The file is immediately created before releasing the lock */
                fs = new FileStream(path, System.IO.FileMode.Create);
            }
            #endregion
            #region Setup ProgressBar
            FileToSend fts = new FileToSend(path, "", fileSize);
            fts.AddToPanel(Program.mainbox.ProgressBarsInPanel);
            #endregion
            #region Show BolloonTip
            string tip;
            lock (Program.users) {
                try {
                    tip = Program.users[((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()].publicName;
                }
                catch (KeyNotFoundException e) {
                    //TODO maybe ask for name?
                    tip = "An user";
                }
            }
            tip += " is sending the ";
            tip += type == Program.FILE ? ("file '" + filename) : ("folder '" + Path.GetFileNameWithoutExtension(filename));
            tip += "'";
            Program.trayIcon.ShowBalloonTip(500, "Jubilant Waffle", tip, ToolTipIcon.None);
            #endregion
            #region Receive file
            data = new byte[Program.bufferSize];
            long alreadyReceived = 0;
            try {
                while (alreadyReceived < fileSize && !fts.cancel) {
                    sizeOfLastRead = client.GetStream().Read(data, 0, (int)Math.Min(Program.bufferSize, fileSize - alreadyReceived));
                    if (sizeOfLastRead == 0) {
                        throw new IOException();
                        Debug.Write("Impossible receiving file, user canceled transfer or disconnected?");
                        return;
                    }
                    alreadyReceived += sizeOfLastRead;
                    Debug.WriteLine("Sent " + alreadyReceived.ToString() + "B out of " + fileSize.ToString() + "B");
                    fs.Write(data, 0, sizeOfLastRead);
                    fts.UpdateProgress(alreadyReceived);
                }
            }
            catch (IOException) {
                // User cancelled the transfer or disconnected
                fts.Error();
                fs.Close();
                File.Delete(path);
                return;
            }
            client.Close();
            fs.Close();
            if (fts.cancel) {
                File.Delete(path);
                return;
            }
            #endregion
            #region Unzip
            if (type == Program.DIRECTORY) {
                /* Create a folder to extract the zip  in the %AppData%\temp directory*/
                Random random = new Random();
                string extract_folder;
                do {
                    extract_folder = "";
                    for (int i = 0; i < 20; i++) {
                        extract_folder += Convert.ToChar(random.Next() % 26 + Convert.ToInt32('A')).ToString();
                    }
                }
                while (Directory.Exists(Program.AppDataFolder + @"\temp\" + extract_folder));
                /* Extract the zip */
                ZipFile.ExtractToDirectory(path, Program.AppDataFolder + @"\temp\" + extract_folder);
                /* Move the content to the target directory. The for iterate on all the directory in the temp folder, but there should be only one */
                foreach (var dir in Directory.GetDirectories(Program.AppDataFolder + @"\temp\" + extract_folder)) {
                    /* Manage filename conflicts */
                    string dest = Path.GetDirectoryName(path) + @"\" + Path.GetFileName(dir);
                    if (Directory.Exists(dest)) {
                        string noEx = dest;                                       // Full path 
                        int i = 1;                                                // File number
                        while (Directory.Exists(dest)) {
                            dest = noEx + "(" + i.ToString() + ")";
                            i++;
                        }
                    }
                    /* Move directory to the target dir
                     * Directory.Move only works if temp folder and target folder are on the same volume 
                     */
                    try {
                        if (Directory.GetDirectoryRoot(dir) == Directory.GetDirectoryRoot(dest)) {
                            Directory.Move(dir, dest);
                        }
                        else {
                            void RecursiveCopy(string src_folder, string dest_folder)
                            {
                                foreach (string file in Directory.GetFiles(src_folder)) {
                                    File.Copy(file, dest_folder + @"\" + Path.GetFileName(file));
                                }
                                foreach (string folder in Directory.GetDirectories(src_folder)) {
                                    Directory.CreateDirectory(dest_folder + @"\" + Path.GetFileName(folder));
                                    RecursiveCopy(folder, dest_folder + @"\" + Path.GetFileName(folder));
                                }
                            }
                            RecursiveCopy(dir, dest);
                        }
                    }
                    catch (Exception e) {
                        return;
                    }
                }
                File.Delete(path);
                Directory.Delete(Program.AppDataFolder + @"\temp\" + extract_folder);
            }
            #endregion
        }
        

    }
}
