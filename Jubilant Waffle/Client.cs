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
        System.Collections.Generic.Dictionary<String, User> users;
        System.Collections.Generic.LinkedList<FileToSend> files;
        System.Net.Sockets.UdpClient udp;
        /* Pipe used to communicate with other istances (i.e. right click send) */
        System.IO.Pipes.NamedPipeServerStream nps;
        bool _cancelCurrent = false; // This is used to undo the current transfer
        string defaultImagePath; //Use this image if none is selected
        public Client(int port) {

            System.Diagnostics.Debug.WriteLine("Client");
            udp = new System.Net.Sockets.UdpClient(port);
            users = new System.Collections.Generic.Dictionary<string, User>();
            files = new System.Collections.Generic.LinkedList<FileToSend>();
            #region Initialize pipe
            try {
                var pSecurity = new System.IO.Pipes.PipeSecurity();
                pSecurity.AddAccessRule(new System.IO.Pipes.PipeAccessRule("Everyone",
                    System.IO.Pipes.PipeAccessRights.ReadWrite,
                    System.Security.AccessControl.AccessControlType.Allow));
                nps = new System.IO.Pipes.NamedPipeServerStream("Jubilant_Waffle",
                    System.IO.Pipes.PipeDirection.InOut,
                    1,
                    System.IO.Pipes.PipeTransmissionMode.Byte,
                    System.IO.Pipes.PipeOptions.None,
                    512, 512,
                    pSecurity,
                    System.IO.HandleInheritability.None
                    );
            }
            catch (Exception e) {
                //TODO handle error
                MessageBox.Show(e.Message);
            }
            #endregion
            #region Initialize Form
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point((Screen.PrimaryScreen.WorkingArea.Width) / 2 - this.Width,
                                   (Screen.PrimaryScreen.WorkingArea.Height) / 2 - this.Width);
            this.ShowInTaskbar = false;
            InitializeComponent();

            /* Image ListView */
            defaultImagePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + @"\default-user-image.png";
            #endregion
            #region Test Users
            users.Add("192.168.1.1", new User("Mario", "192.168.1.1"));
            users.Add("192.168.1.2", new User("Luigi", "192.168.1.2"));
            users.Add("192.168.1.3", new User("Antonio", "192.168.1.3"));
            #endregion

            #region Execute routines
            System.Threading.Thread ConsumeFileListThread = new System.Threading.Thread(() => ConsumeFileList());
            System.Threading.Thread ListenForConnectionsThread = new System.Threading.Thread(() => ListenForConnections());
            System.Threading.Thread ReadPipeThread = new System.Threading.Thread(() => ReadPipe());
            ConsumeFileListThread.Name = "ConsumeFileList";
            ListenForConnectionsThread.Name = "ListenForConnections";
            ReadPipeThread.Name = "ReadPipe";
            ConsumeFileListThread.Start();
            ListenForConnectionsThread.Start();
            ReadPipeThread.Start();
            #endregion
        }
        
        static public void EnqueueMessageInPipe(string msg) {
            /// static method that wrap the creating of a named pipe for communicate with main instance
            /// Data are pushed tp the pipe in the following order.
            ///     - Lenght of path string on 4 bytes
            ///     - Path
            System.IO.Pipes.NamedPipeClientStream pipe;
            try {
                pipe = new System.IO.Pipes.NamedPipeClientStream("Jubilant_Waffle");
                pipe.Connect();
            }
            catch (Exception e) {
                //TODO Handle error
                MessageBox.Show(e.Message);
                return;
            }
            if (!pipe.IsConnected) {
                //TODO Handle error
                return;
            }   
            byte[] data;
            /*
            #region Push IP
            data = System.BitConverter.GetBytes(ip.Length);
            pipe.Write(data, 0, data.Length);
            data = System.Text.Encoding.ASCII.GetBytes(ip);
            pipe.Write(data, 0, data.Length);
            #endregion
            */

            #region Push path
            data = System.BitConverter.GetBytes(msg.Length);
            pipe.Write(data, 0, data.Length);
            data = System.Text.Encoding.ASCII.GetBytes(msg);
            pipe.Write(data, 0, data.Length);
            #endregion
        }
        private void ReadPipe() {
            /// This method wait for connection on pipe and fill the list of file to be sent
            /// Data are pushed into the pipe using the EnqueueMessageInPipe method 
            /// and thus are read in the following order
            ///     - Lenght of path string on 4 bytes
            ///     - Path
            System.Diagnostics.Debug.WriteLine("Read Pipe");
            byte[] len = new byte[4];
            byte[] data;
            int lenght;
            string path;
            FileToSend fts;
            while (true) {
                nps.WaitForConnection();
                /*
                #region Read IP address
                nps.Read(len, 0, 4);
                lenght = System.BitConverter.ToInt32(len, 0);
                data = new byte[lenght];
                nps.Read(data, 0, lenght);
                ip = System.Text.Encoding.ASCII.GetString(data);
                #endregion
                */
                #region Read path
                nps.Read(len, 0, 4);
                lenght = System.BitConverter.ToInt32(len, 0);
                data = new byte[lenght];
                nps.Read(data, 0, lenght);
                path = System.Text.Encoding.ASCII.GetString(data);
                #endregion
                #region Ask to select a user
                //TODO implement GUI
                #endregion
                #region Push new file
                fts = new FileToSend();
                fts.path = path;
                lock (files) {
                    files.AddLast(fts);
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
                SendFile(fts.path, new System.Net.IPEndPoint(System.Net.IPAddress.Parse(fts.ip), 20000));
            }
        }
        private void SendFile(string path, System.Net.IPEndPoint iPEndPoint) {
            /// The file is sent using the following sintax
            ///     - Control message 'FILE!'
            ///     - File name lenght on 4 bytes
            ///     - File name
            ///     - File size on 8 bytes
            ///     - File data
            ///  
            #region variables
            System.Net.Sockets.TcpClient dataChannel = new System.Net.Sockets.TcpClient(); // The tcp client used to send data 
            dataChannel.ReceiveTimeout = dataChannel.SendTimeout = 2000;
            System.IO.FileStream fs; //The file stream to be send 
            byte[] data; // buffer for sockets
            int nameLenght;
            long fileSize; // The size of the file to be sent
            long dataSent = 0; // The amount of data already sent
            #endregion
            #region Open connection
            data = System.Text.Encoding.ASCII.GetBytes("FILE!");
            try {
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
                        if (!users.ContainsKey(endpoint.Address.ToString())) {
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
                        if (users.ContainsKey(endpoint.Address.ToString())) {
                            users.Remove(endpoint.Address.ToString());
                        }
                        break;

                }
            }
        }
        private void AddNewUser(System.Net.IPAddress userAddress) {
            #region Connect to user to ask info
            System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient();
            tcp.SendTimeout = tcp.ReceiveTimeout = 2000;
            try {
                tcp.Connect(new System.Net.IPEndPoint(userAddress, 20000));
            }
            catch (System.Net.Sockets.SocketException) {
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
            lock (users) {
                if (msg == "MARIO") {
                    users.Add(userAddress.ToString(), new User(name, userAddress.ToString(), "user_pic/" + userAddress.ToString() + ".png"));
                }
                else {
                    users.Add(userAddress.ToString(), new User(name, userAddress.ToString()));
                }
            }
            #endregion
        }
    }
}
