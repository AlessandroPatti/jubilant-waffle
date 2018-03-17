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
    public struct FileToSend {
        public string path;
        public string ip;
    }
    public partial class Client : Form {
        /* Map of the users known. It indexed by IP address since each connection brings the IP */
        System.Collections.Generic.LinkedList<FileToSend> files;
        System.Net.Sockets.UdpClient udp;
        /* Pipe used to communicate with other istances (i.e. right click send) */
        bool _cancelCurrent = false; // This is used to undo the current transfer
        string defaultImagePath; //Use this image if none is selected
        System.Net.Sockets.TcpListener instancesListener;
        const int port = 20000;
        const int timeout = 200000;
        public Client() {
            System.Diagnostics.Debug.WriteLine("Client");
            udp = new System.Net.Sockets.UdpClient(port);
            files = new System.Collections.Generic.LinkedList<FileToSend>();
            #region Initialize Form
            InitializeComponent();
            this.FormClosing += PreventClose;
            /* Image ListView */
            defaultImagePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + @"\default-user-image.png";
            #endregion
            #region Initiliaze socket to list for local connections
            instancesListener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, port + 1);
            #endregion
            #region Calling show to fire load
            Size tmp = this.Size;
            this.Size = new Size(0, 0);
            this.Show();
            this.Hide();
            this.Size = tmp;
            #endregion
            #region Execute routines
            System.Threading.Thread ConsumeFileListThread = new System.Threading.Thread(() => ConsumeFileList());
            System.Threading.Thread ListenForConnectionsThread = new System.Threading.Thread(() => ListenForConnections());
            System.Threading.Thread ReadMQThread = new System.Threading.Thread(() => ReadMS());
            ConsumeFileListThread.Name = "ConsumeFileList";
            ListenForConnectionsThread.Name = "ListenForConnections";
            ReadMQThread.Name = "ReadMQ";
            ConsumeFileListThread.Start();
            ListenForConnectionsThread.Start();
            ReadMQThread.Start();
            #endregion
        }


        private void PreventClose(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
        }

        delegate void UpdateListCallback();
        private void UpdateList() {
            if (this.InvokeRequired) {
                UpdateListCallback callback = new UpdateListCallback(UpdateList);
                UserListView.Invoke(callback);
            }
            else {
                ImageList imgl = new ImageList();
                Image img;
                imgl.ImageSize = new Size(200, 200);
                UserListView.Clear();
                lock (Program.users) {
                    foreach (User u in Program.users.Values) {
                        img = Image.FromFile(u.imagePath != null ? u.imagePath : defaultImagePath);
                        imgl.Images.Add(u.ip, img);
                    }
                    UserListView.LargeImageList = imgl;
                    var i = 0;
                    foreach (User u in Program.users.Values) {
                        UserListView.Items.Add(u.ip, u.name, i++);
                        UserListView.Items[i - 1].ImageKey = u.ip;
                    }
                }
                this.Show();
                this.ShowInTaskbar = true;
            }
        }

        static public void EnqueueMessage(string msg) {
            System.Net.Sockets.TcpClient client;
            byte[] data;

            client = new System.Net.Sockets.TcpClient();
            client.Connect(System.Net.IPAddress.Loopback, port + 1);
            data = System.BitConverter.GetBytes(msg.Length);
            client.GetStream().Write(data, 0, data.Length);
            data = System.Text.Encoding.ASCII.GetBytes(msg);
            client.GetStream().Write(data, 0, data.Length);
            try {
                client.GetStream().Read(data, 0, 1);
            }
            catch {
                //TODO handle
                return;
            }
        }
        private void ReadMS() {
            string msg = "", path = "";
            byte[] data, len = new byte[4];
            int lenght, count;
            System.Net.Sockets.TcpClient client;
            instancesListener.Start();
            System.IO.Pipes.NamedPipeServerStream npss;
            while (true) {
                #region read message from queue
                client = instancesListener.AcceptTcpClient();
                client.GetStream().Read(len, 0, len.Length);
                lenght = System.BitConverter.ToInt32(len, 0);
                data = new byte[lenght];
                client.GetStream().Read(data, 0, data.Length);
                path = System.Text.Encoding.ASCII.GetString(data);
                client.Close();
                #endregion
                #region Ask user
                npss = new System.IO.Pipes.NamedPipeServerStream("JubilantWaffleInternal");
                UpdateList();
                npss.WaitForConnection();
                /* Read response 
                 *
                 * It is sent as
                 *      - Count: number of users selected
                 *      - length 1st ip address
                 *      - 1st ip address
                 *      - ...
                 */

                npss.Read(len, 0, len.Length);
                count = System.BitConverter.ToInt32(len, 0);
                #endregion
                #region Read Users
                List<FileToSend> tmp = new List<FileToSend>();
                for (var i = 0; i < count; i++) {
                    npss.Read(len, 0, len.Length);
                    lenght = System.BitConverter.ToInt32(len, 0);
                    data = new byte[lenght];
                    npss.Read(data, 0, data.Length);
                    msg = System.Text.Encoding.ASCII.GetString(data);
                    System.Diagnostics.Debug.WriteLine(path);
                    tmp.Add(new FileToSend { path = path, ip = msg });
                }
                npss.Close();
                #endregion
                #region Enqueue files

                if (count > 0)
                    Program.mainbox.AddProgressBarOut(path + msg);
                lock (files) {
                    foreach (FileToSend file in tmp) {
                        files.AddLast(file);
                    }
                    System.Threading.Monitor.PulseAll(files);
                }
                #endregion
            }
        }
        private void ConsumeFileList() {

            System.Diagnostics.Debug.WriteLine("Consume File List");
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
                SendFile(fts.path, new System.Net.IPEndPoint(System.Net.IPAddress.Parse(fts.ip), port));
            }
        }
        private void SendFile(string path, System.Net.IPEndPoint IPEndPoint) {
            /// The file is sent using the following sintax
            ///     - Control message 'FILE!'
            ///     - File name lenght on 4 bytes
            ///     - File name
            ///     - File size on 8 bytes
            ///     - File data
            ///  
            #region variables
            System.Net.Sockets.TcpClient dataChannel = new System.Net.Sockets.TcpClient(); // The tcp client used to send data 
            dataChannel.ReceiveTimeout = dataChannel.SendTimeout = timeout;
            System.IO.FileStream fs; //The file stream to be send 
            byte[] data; // buffer for sockets
            int nameLenght;
            long fileSize; // The size of the file to be sent
            long dataSent = 0; // The amount of data already sent
            ProgressBar pbar;
            #endregion
            #region Open connection
            data = System.Text.Encoding.ASCII.GetBytes("FILE!");
            try {
                dataChannel.Connect(IPEndPoint);
                dataChannel.GetStream().Write(data, 0, 5);
            }
            catch (System.Net.Sockets.SocketException) {
                /* Could not connect to the host, something went wrong. File will not be sent */
                System.Console.Write("Impossible send file, connection unsuccessful");
                return;
            }
            #endregion
            //TODO eventually add a connection for control channel
            #region Send file name lenght
            nameLenght = System.IO.Path.GetFileName(path).Length;
            data = System.BitConverter.GetBytes(nameLenght);
            try {
                dataChannel.GetStream().Write(data, 0, data.Length);
            }
            catch (System.Net.Sockets.SocketException) {
                /* Could not connect to the host, something went wrong. File will not be sent */
                System.Console.Write("Impossible send file, failed sending file name lenght");
                return;
            }
            #endregion
            #region Send file name
            data = System.Text.Encoding.ASCII.GetBytes(System.IO.Path.GetFileName(path));
            try {
                dataChannel.GetStream().Write(data, 0, data.Length);
            }
            catch (System.Net.Sockets.SocketException) {
                /* Could not connect to the host, something went wrong. File will not be sent */
                System.Console.Write("Impossible send file, failed sending file name");
                return;
            }
            #endregion
            #region Read response
            //TODO
            #endregion
            #region Zip
            //TODO zip the file
            #endregion
            #region Send file length
            fileSize = (new System.IO.FileInfo(path)).Length;//TODO update with size of archive when compression is implemented
            data = System.BitConverter.GetBytes(fileSize);
            try {
                dataChannel.GetStream().Write(data, 0, data.Length);
            }
            catch (System.Net.Sockets.SocketException) {
                /* Could not connect to the host, something went wrong. File will not be sent */
                System.Console.Write("Impossible send file, connection unsuccessful");
                return;
            }
            #endregion
            #region Set Progress Bar
            pbar = Program.mainbox.ProgressBarsOut[path + IPEndPoint.Address.ToString()];
            pbar.Maximum = (int)(fileSize / (1024 * 1024));
            pbar.Minimum = 0;
            pbar.Step = pbar.Maximum / 4;
            #endregion;
            #region Send file
            /* Open the file */
            try {
                fs = System.IO.File.OpenRead(path);
            }
            catch (System.IO.FileNotFoundException) {
                /* Could not find the file. File will not be sent */
                System.Console.Write("Impossible send file, file not found");
                return;
            }

            /* Send the file */
            data = new byte[4 * 1024 * 1024];
            while (dataSent < fileSize && !_cancelCurrent) {
                int sizeOfLastRead = 0;
                try {
                    sizeOfLastRead = fs.Read(data, 0, (int)System.Math.Min(fileSize - dataSent, (long)data.Length));
                }
                catch {
                    /* Could not read the file. File will not be sent */
                    System.Console.Write("Impossible send file, error will reading");
                    return;
                }
                dataChannel.GetStream().Write(data, 0, sizeOfLastRead);
                pbar.PerformStep();
            }
            /* reset cancelCurrent. It assures that if it has been sent, it wont be active for next file in the list */
            _cancelCurrent = false;
            #endregion
        }

        private void ListenForConnections() {
            /// Listen for user in/out in the LAN
            /// 
            System.Diagnostics.Debug.WriteLine("Listen for connections...");
            while (true) {
                System.Net.IPEndPoint endpoint = new System.Net.IPEndPoint(0, 0); // The endpoint will identify the user that sent the message
                byte[] data = udp.Receive(ref endpoint); //Wait for a new message. It is blocking
                string msg = System.Text.Encoding.ASCII.GetString(data);
                switch (msg) {
                    /* New user connection */
                    case "HELLO":
                        if (endpoint.Address.ToString() == Program.self.ip)
                            continue;
                        if (!Program.users.ContainsKey(endpoint.Address.ToString())) {
                            /* 
                             * Add only if user is not already present.
                             * Excuted on the same thread will manage only a connection at a time
                             * but do not require concurrency control on the list of users?
                             */
                            AddNewUser(endpoint.Address);
                        }
                        break;
                    /* An user is leaving */
                    case "BYE!!":
                        if (Program.users.ContainsKey(endpoint.Address.ToString())) {
                            Program.users.Remove(endpoint.Address.ToString());
                        }
                        break;

                }
            }
        }
        private void AddNewUser(System.Net.IPAddress userAddress) {
            #region Connect to user to ask info
            System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient();
            tcp.SendTimeout = tcp.ReceiveTimeout = timeout;
            try {
                tcp.Connect(new System.Net.IPEndPoint(userAddress, port));
            }
            catch {
                /* Could not connect to the host, something went wrong. Nothing will happen */
                System.Console.Write("Impossible add new user, connection unsuccessful");
                return;
            }
            #endregion
            #region Ask for info
            try {
                tcp.Client.Send(System.Text.Encoding.ASCII.GetBytes("WHO??"));
            }
            catch (System.Net.Sockets.SocketException) {
                /* Could not connect to the host, something went wrong. Nothing will happen */
                System.Console.Write("Impossible add new user, request unsuccessful");
                return;
            }
            #endregion
            #region Get reply message
            /* The reply message is
             *      - MARIO  when the message contains the image. In this case 
             *          the message will have this format
             *              - 4 byte of name lenght
             *              - name
             *              - 8 byte for image lengt
             *              - image
             *      - LUIGI when the user do not have any image. In this case 
             *          the message will have this format
             *              - 4 byte of name lenght
             *              - name
             */
            byte[] msgByte = new byte[5];
            string msg;
            try {
                tcp.GetStream().Read(msgByte, 0, 5);
            }
            catch (System.Net.Sockets.SocketException) {
                /* Could not connect to the host, something went wrong. Nothing will happen */
                System.Console.Write("Impossible add new user, reply unsuccessful");
                return;
            }
            msg = System.Text.Encoding.ASCII.GetString(msgByte);
            #endregion
            #region Get Name

            /* 
             * First read the leght of the name 
             */
            Int32 nameLenght;
            byte[] lenght = new byte[4];
            byte[] nameByte;
            string name;
            try {
                tcp.GetStream().Read(lenght, 0, 4);
            }
            catch (System.Net.Sockets.SocketException) {
                /* Could not connect to the host, something went wrong. Nothing will happen */
                System.Console.Write("Impossible add new user, name lenght unsuccessful");
                return;
            }
            nameLenght = System.BitConverter.ToInt32(lenght, 0); //TODO how to manage Big and little endian?

            /* 
             * Read the actual name 
             */
            nameByte = new byte[nameLenght];
            try {
                tcp.GetStream().Read(nameByte, 0, nameLenght);
            }
            catch (System.Net.Sockets.SocketException) {
                /* Could not connect to the host, something went wrong. Nothing will happen */
                System.Console.Write("Impossible add new user, name unsuccessful");
                return;
            }
            name = System.Text.Encoding.ASCII.GetString(nameByte);
            #endregion
            #region Eventually get image and store it into disk
            if (msg == "MARIO") {
                /* 
                 * First read the leght of the image file 
                 */
                long imageLenght;
                byte[] image;
                try {
                    tcp.GetStream().Read(lenght, 0, 8);
                }
                catch (System.Net.Sockets.SocketException) {
                    /* Could not connect to the host, something went wrong. Nothing will happen */
                    System.Console.Write("Impossible add new user, image lenght unsuccessful");
                    return;
                }
                imageLenght = System.BitConverter.ToInt64(lenght, 0); //TODO how to manage big and little endian?

                /* 
                 * Read the actual image file 
                 */
                image = new byte[imageLenght];
                try {
                    tcp.GetStream().Read(image, 0, (Int32)imageLenght);  //TODO Warning! Casting to int may cause lost of MSBs! Introduce image size limit?
                }
                catch (System.Net.Sockets.SocketException) {
                    /* Could not connect to the host, something went wrong. Nothing will happen */
                    System.Console.Write("Impossible add new user, image unsuccessful");
                    return;
                }

                /*
                 * Write the image file to disk
                 */
                System.IO.FileStream fs = System.IO.File.Create("user_pic/" + userAddress.ToString() + ".png");
                fs.Write(image, 0, image.Length);
                fs.Close();
            }
            #endregion
            #region Add the user to the list of known
            lock (Program.users) {
                if (msg == "MARIO") {
                    Program.users.Add(userAddress.ToString(), new User(name, userAddress.ToString(), "user_pic/" + userAddress.ToString() + ".png"));
                }
                else {
                    Program.users.Add(userAddress.ToString(), new User(name, userAddress.ToString()));
                }
            }
            #endregion
        }

        private void ConfirmSend(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                byte[] data;
                System.IO.Pipes.NamedPipeClientStream npcs = new System.IO.Pipes.NamedPipeClientStream("JubilantWaffleInternal");
                npcs.Connect();
                data = System.BitConverter.GetBytes(UserListView.SelectedItems.Count);
                npcs.Write(data, 0, data.Length);
                foreach (ListViewItem item in UserListView.SelectedItems) {
                    data = System.BitConverter.GetBytes(item.ImageKey.Length);
                    npcs.Write(data, 0, data.Length);
                    data = System.Text.Encoding.ASCII.GetBytes(item.ImageKey);
                    npcs.Write(data, 0, data.Length);
                }
                npcs.WaitForPipeDrain();
                npcs.Close();
                this.Hide();
                this.ShowInTaskbar = false;
            }
        }
        private void UndoSend(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                byte[] data;
                System.IO.Pipes.NamedPipeClientStream npcs = new System.IO.Pipes.NamedPipeClientStream("JubilantWaffleInternal");
                npcs.Connect();
                data = System.BitConverter.GetBytes(0);
                npcs.Write(data, 0, data.Length);
                npcs.WaitForPipeDrain();
                npcs.Close();
                this.Hide();
                this.ShowInTaskbar = false;
            }
        }
    }
}
