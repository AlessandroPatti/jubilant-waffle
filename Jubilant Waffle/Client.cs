using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Jubilant_Waffle {
    public partial class Client : Form {
        LinkedList<FileToSend> files;                                       // Will store all the files enqueued for sending
        System.Net.Sockets.UdpClient udp;                                   // The udp socket used to listen for new user connecting
        System.Net.Sockets.TcpListener instancesListener;                   // Used by other istances of the same program to enqueue a file that as been right clicked.


        public Client() {
            udp = new UdpClient(Program.port);
            files = new LinkedList<FileToSend>();
            InitializeComponent();
            #region Execute routines
            System.Threading.Thread ConsumeFileListThread = new System.Threading.Thread(() => ConsumeFileList());
            System.Threading.Thread ListenForConnectionsThread = new System.Threading.Thread(() => ListenForConnections());
            System.Threading.Thread ReadMQThread = new System.Threading.Thread(() => ReadMS());
#if DEBUG
            ConsumeFileListThread.Name = "ConsumeFileList";
            ListenForConnectionsThread.Name = "ListenForConnections";
            ReadMQThread.Name = "ReadMQ";
#endif
            ConsumeFileListThread.Start();
            ListenForConnectionsThread.Start();
            ReadMQThread.Start();
            #endregion
        }

        delegate void UpdateListCallback();
        private void UpdateList() {
            ///<summary>
            /// Update the content of the form listing all the known users with the relative images.
            /// At the end of the method, the form is shown.
            ///</summary>

            if (this.InvokeRequired) {
                /* The current thread is not the owner of the form so the owner
                 * is asked to call the method to perform the modification
                 */
                UpdateListCallback callback = new UpdateListCallback(UpdateList);
                UserListView.Invoke(callback);
            }
            else {
                ImageList imgl = new ImageList();
                Image img;
                imgl.ImageSize = new Size(150, 150);                            // The size of the pictures of the user. 
                imgl.ColorDepth = ColorDepth.Depth32Bit;                        // The quality of the images. 32bit is the maximum.
                UserListView.Clear();                                           // Clear out the current list of user. It may contains users that are now disconnected.
                lock (Program.users) {
                    #region AddImages
                    foreach (User u in Program.users.Values) {
                        img = Image.FromFile(u.imagePath ?? Program.defaultImagePath);
                        imgl.Images.Add(u.ip, img);
                    }
                    UserListView.LargeImageList = imgl;
                    #endregion
                    #region Add names
                    /* Each image in the image list has an index. Since the list of user is scanned 
                     * in the same order, each i-th name in the for will correspond the the i-th image
                     * scanned before, so to get the right index of the current name and image is
                     * enough to use a counter (the local variable "i") 
                     */
                    var i = 0;
                    foreach (User u in Program.users.Values) {
                        UserListView.Items.Add(u.ip, u.publicName, i++);
                        /* The image key can be retrived when the item is selected. 
                         * By storing the ip of the corresponding user, it will be possible to get all
                         * the information regarding him since the user are stored in a dictionary
                         * where the key is the ip address.
                         */
                        UserListView.Items[i - 1].ImageKey = u.ip;
                    }
                    #endregion
                }
            }
            /* The confirm button has to be enabled only if at least one item is selected */
            Confirm.Enabled = false;
        }
        private void EnableDisableConfirmation(object sender, ListViewItemSelectionChangedEventArgs e) {
            /// <summary>
            /// The confirm button has to be enabled only if at least one item is selected
            /// </summary>
            Confirm.Enabled = UserListView.SelectedItems.Count != 0;
        }

        static public void EnqueueMessage(string msg) {
            /// <summary>
            /// The main instance is listening on the tcp port 20001 for other instances of the application.
            /// This method can be used to send a message (i.e. the path of the file) from anothre instance to the main one.
            /// </summary>

            byte[] data;
            TcpClient client = new TcpClient();
            client.Connect(System.Net.IPAddress.Loopback, Program.port + 1);

            /* First send the lenght of the string on 4 bytes, then the string itself */
            data = BitConverter.GetBytes(msg.Length);
            client.GetStream().Write(data, 0, data.Length);
            data = Encoding.ASCII.GetBytes(msg);
            client.GetStream().Write(data, 0, data.Length);
        }
        private void ReadMS() {
            /// <summary>
            /// This routine waits for connection of other instances on the tcp port 200001.
            /// When a new connection occurs, first it reads from the socket the path of the file
            /// then lets the user select to whom the file has to be sent, and finally enqueue the file.
            /// </summary>
            string path;                        // The path of the file enqueued
            byte[] data;                        // Buffer for the socket
            byte[] len = new byte[4];           // Buffer for the socket
            int lenght;                         // Support variable that will store the lenght of the path
            DialogResult res;                   // The result of the dialog in which the user can select users and confirm or cancel the transfer
            TcpClient client;                   // The client used to communicate with the other instance

            instancesListener = new TcpListener(System.Net.IPAddress.Loopback, Program.port + 1);
            instancesListener.Start();
            while (true) {
                #region Read message from the socket
                client = instancesListener.AcceptTcpClient();
                client.GetStream().Read(len, 0, len.Length);
                lenght = BitConverter.ToInt32(len, 0);
                data = new byte[lenght];
                client.GetStream().Read(data, 0, data.Length);
                path = Encoding.ASCII.GetString(data);
                client.Close();
                #endregion
                /* The path send using the right-click on the file sometimes is shortened
                 * automatically by windows. Altought the path is still valid, the name of the file,
                 * which as to be sent to the remote host, might be modified (e.g. example_file.txt -> EXA~1.txt)
                 * The followng line aims at fixing the problem by retriving the full path from the FS.
                 */
                path = (new FileInfo(path)).FullName;
                #region Ask user
                UpdateList();
                res = ShowDialog();
                #endregion
                #region Eventually enqueue file/users
                if (res == DialogResult.OK) {
                    /* File-user pair are first stored in a temporary list while the object are created and then
                     * pushed all the list togheter to avoid multiple pulse on the Monitor
                     */
                    List<FileToSend> tmp = new List<FileToSend>();
                    FileToSend fts;
                    foreach (ListViewItem item in UserListView.SelectedItems) {
                        fts = new FileToSend(path, item.ImageKey);
                        fts.AddToPanel(Program.mainbox.ProgressBarsOutPanel);
                        tmp.Add(fts);
                    }
                    lock (files) {
                        foreach (FileToSend file in tmp) {
                            files.AddLast(file);
                        }
                        System.Threading.Monitor.PulseAll(files);
                    }
                }
                #endregion
            }
        }
        private void ConsumeFileList() {
            /// <summary>
            /// This method sleeps on a Monitor (i.e. condition variable) until a new file is pushed in the queue.
            /// Once awake, it will rest awake until all files in the list have been consumed.
            /// </summary>
            FileToSend fts;
            while (true) {
                lock (files) {
                    /* If there are no files pending, wait until notification received */
                    while (files.Count == 0) {
                        System.Threading.Monitor.Wait(files);
                    }
                    /* Get first in the list and remove it */
                    fts = files.First.Value;
                    files.RemoveFirst();
                }
                if (!fts.cancel)
                    /* The follonwing method can be executed on a separate thread to send multiple file at a time */
                    SendFile(fts, new System.Net.IPEndPoint(System.Net.IPAddress.Parse(fts.ip), Program.port));
            }
        }
        private void SendFile(FileToSend fts, IPEndPoint IPEndPoint) {
            /// <summary>
            /// Send the file to a remote host. It sends first the name and the lenght of the file and
            /// wait for a response from the remote party. If it accepts the file, the files is zipped and sent.
            /// </summary>
            #region variables
            TcpClient tcp = new TcpClient();                                    // The tcp client used to send data 

            tcp.ReceiveTimeout = tcp.SendTimeout = Program.timeout;             // Set the timeout for the connection. if the timeout expires, it is asumend that the remote host has disconnected
            FileStream fs;                                            // The file stream to be send 
            byte[] data;
            long dataSent = 0;                                                  // The amount of data already sent
            #endregion
            #region Open connection
            try {
                tcp.Connect(IPEndPoint);
                tcp.GetStream().WriteByte(Program.FILE);
                #endregion
                #region Send file name lenght
                data = BitConverter.GetBytes(Path.GetFileName(fts.path).Length);
                tcp.GetStream().Write(data, 0, data.Length);
                #endregion
                #region Send file name
                data = Encoding.ASCII.GetBytes(Path.GetFileName(fts.path));
                tcp.GetStream().Write(data, 0, data.Length);
                #endregion
                #region Send file size
                data = BitConverter.GetBytes(fts.fileSize);
                tcp.GetStream().Write(data, 0, data.Length);
                #endregion
                #region Read response
                tcp.GetStream().Read(data, 0, 1);
            }
            catch (IOException) {
                Debug.Write("Impossible send file, socket timeout expired");
                //TODO Inform the user that transfer has failed
                return;
            }
            if (data[0] == Program.TRANSFER_DENY) {
                /* The remote host refused the file */
                //TODO Inform the user that transfer has failed
                return;
            }
            #endregion
            #region Zip
            //TODO zip the file
            #endregion
            #region Send file
            /* Open the file */
            try {
                fs = File.OpenRead(fts.path);
            }
            catch (FileNotFoundException) {
                Debug.Write("Impossible send file, file not found");
                return;
            }

            /* Send the file */
            data = new byte[Program.bufferSize];
            int sizeOfLastRead = 0;
            while (dataSent < fts.fileSize && !fts.cancel) {
                try {
                    sizeOfLastRead = fs.Read(data, 0, (int)Math.Min(fts.fileSize - dataSent, data.LongLength));
                }
                catch (IOException) {
                    Debug.Write("Impossible send file, error will reading");
                    return;
                }
                try {
                    tcp.GetStream().Write(data, 0, sizeOfLastRead);
                }
                catch (IOException e) {

                    Debug.Write("Impossible send file, error will reading");
                    return;
                }
                dataSent += sizeOfLastRead;
                Debug.WriteLine("Sent " + dataSent.ToString() + "B out of " + fts.fileSize.ToString() + "B");
                fts.UpdateProgress(dataSent);
            }
            tcp.Close();
            #endregion
        }

        private void ListenForConnections() {
            /// <summary>
            /// Listen on udp port 20000 for incoming packet. Another application will 
            /// produce UDP traffic only to announce itself on the net when online, 
            /// or to inform other parties that is disconection when going offline
            /// </summary>
            /// 
            /// 
            IPEndPoint endpoint = new IPEndPoint(0, 0);             // The endpoint will identify the user that sent the message
            byte[] msg;                                             // Buffer for udp packets
            while (true) {
                msg = udp.Receive(ref endpoint);
                switch (msg[0]) {
                    case Program.HELLO:
                        if (endpoint.Address.ToString() == Program.self.ip) {
                            continue;
                            /* Since the announcement is done in broadcast, if the application status is "online"
                             * it can receive back (depending on the switch) the broadcast packets it sends.
                             * Those packet will be discarded.
                             */
                        }
                        if (!Program.users.ContainsKey(endpoint.Address.ToString())) {
                            /* The user is not know, thus it has to be added.
                             */
                            try {
                                AddNewUser(endpoint.Address);
                            }
                            catch (IOException) {
                                /* IO exception are launched in the method if the user disconnect or
                                 * if it was not possible to store the informations
                                 */
                                //TODO how to discen the two case?
                            }
                        }
                        break;
                    case Program.BYE:
                        if (Program.users.ContainsKey(endpoint.Address.ToString())) {
                            /* If the user is known it has to be removed from the list of online user
                             * so that it wont appear in the list when the local user tries to send a file
                             */
                            lock (Program.users) {
                                Program.users.Remove(endpoint.Address.ToString());
                            }
                        }
                        break;

                }
            }
        }
        private void AddNewUser(System.Net.IPAddress userAddress) {
            /// <summary>
            /// Connects to the remote host and ask for user's informations (i.e. name and eventually image).
            /// The method can throw IOException when the socket timeout expires.
            /// </summary>

            TcpClient tcp = new TcpClient();
            byte[] data, response;          // Buffer for socket
            string name;                    // Will contain the name of the user
            int lenght;                     // Support variable that is used to store the lenght of the next message (i.e. name lenght)
            string imagepath = null;        // The path of the image file, if it will be received. The name of the file will be <ip_address>.png to ensure that it will be unique. 

            tcp.SendTimeout = tcp.ReceiveTimeout = Program.timeout;

            #region Connect to user and ask info
            tcp.Connect(new IPEndPoint(userAddress, Program.port));
            tcp.GetStream().WriteByte(Program.INFORMATION_REQUEST);
            #endregion
            #region Get reply message
            /* The reply message is always positive. It is used to know if the user will also send an image or not */
            response = new byte[1];
            tcp.GetStream().Read(response, 0, 1);
            #endregion
            #region Get Name
            /* Get name lenght */
            data = new byte[4];
            tcp.GetStream().Read(data, 0, 4);
            lenght = BitConverter.ToInt32(data, 0); //TODO how to manage Big and little endian?
            /* Get name */
            data = new byte[lenght];
            tcp.GetStream().Read(data, 0, data.Length);
            name = Encoding.ASCII.GetString(data);
            #endregion
            #region Eventually get image and store it into disk
            if (response[0] == Program.INFO_WITH_IMAGE) {
                long fileSize;                          // Support variable that is used to store the lenght of the image file

                /* Read file size first */
                data = new byte[8];
                tcp.GetStream().Read(data, 0, data.Length);
                fileSize = BitConverter.ToInt64(data, 0);

                /* Read the actual image file */
                if (!Directory.Exists(Program.AppDataFolder + @"\user_pic")) {
                    /* The user picture will be stored in the the user_pic folder under the AppData folder.
                     * If it does not exists, it should be created. 
                     */
                    Directory.CreateDirectory(Program.AppDataFolder + @"\user_pic");
                }
                data = new byte[Program.bufferSize];
                imagepath = Program.AppDataFolder + @"\user_pic\" + userAddress.ToString() + ".png";
                FileStream fs = new FileStream(imagepath, FileMode.Create);
                long alreadyReceived = 0;
                int sizeOfLastRead = 0;
                while (alreadyReceived < fileSize) {
                    sizeOfLastRead = tcp.GetStream().Read(data, 0, (int)Math.Min(Program.bufferSize, fileSize - alreadyReceived));
                    alreadyReceived += sizeOfLastRead;
                    fs.Write(data, 0, sizeOfLastRead);
                    Debug.WriteLine("Sent " + alreadyReceived.ToString() + "B out of " + fileSize.ToString() + "B");
                }
                fs.Close();
            }
            #endregion
            #region Add the user to the list of known
            lock (Program.users) {
                /* The imagepath variable willbe null if no image has been received. */
                Program.users.Add(userAddress.ToString(), new User(name, userAddress.ToString(), imagepath));
            }
            #endregion
        }

    }
}
