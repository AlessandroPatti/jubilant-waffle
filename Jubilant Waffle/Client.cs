using System;

namespace Jubilant_Waffle {
    public struct FileToSend {
        public string path;
        public string ip;
    }
    class Client {
        /* Map of the users known. It indexed by IP address since each connection brings the IP */
        System.Collections.Generic.Dictionary<String, User> users;
        System.Collections.Generic.LinkedList<FileToSend> files;
        System.Net.Sockets.UdpClient udp;

        bool _cancelCurrent = false; // This is used to undo the current transfer

        public Client() {
            udp = new System.Net.Sockets.UdpClient();
            users = new System.Collections.Generic.Dictionary<string, User>();
            files = new System.Collections.Generic.LinkedList<FileToSend>();
            new System.Threading.Thread(() => ConsumeFileList());
            new System.Threading.Thread(() => ListenForConnections());
        }

        private void ConsumeFileList() {
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
            catch (System.Net.Sockets.SocketException e) {
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
            catch (System.Net.Sockets.SocketException e) {
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
            catch (System.Net.Sockets.SocketException e) {
                /* Could not connect to the host, something went wrong. File will not be sent */
                System.Console.Write("Impossible send file, failed sending file name");
                return;
            }
#endregion
            #region Send file length
            fileSize = (new System.IO.FileInfo(path)).Length;
            data = System.BitConverter.GetBytes(fileSize);
            try {
                dataChannel.GetStream().Write(data, 0, data.Length);
            }
            catch (System.Net.Sockets.SocketException e) {
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
            catch (System.IO.FileNotFoundException e) {
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
            catch (System.Net.Sockets.SocketException e) {
                /* Could not connect to the host, something went wrong. Nothing will happen */
                System.Console.Write("Impossible add new user, connection unsuccessful");
                return;
            }
            #endregion
            #region Ask for info
            try {
                tcp.Client.Send(System.Text.Encoding.ASCII.GetBytes("WHO??"));
            }
            catch (System.Net.Sockets.SocketException e) {
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
            catch (System.Net.Sockets.SocketException e) {
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
            catch (System.Net.Sockets.SocketException e) {
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
            catch (System.Net.Sockets.SocketException e) {
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
                catch (System.Net.Sockets.SocketException e) {
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
                    tcp.GetStream().Read(image, 0, (Int32) imageLenght);  //TODO Warning! Casting to int may cause lost of MSBs! Introduce image size limit?
                }
                catch (System.Net.Sockets.SocketException e) {
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
            if (msg == "MARIO") {
                users.Add(userAddress.ToString(), new User(name, userAddress.ToString(), "user_pic/" + userAddress.ToString() + ".png"));
            }
            else {
                users.Add(userAddress.ToString(), new User(name, userAddress.ToString()));
            }
            #endregion
        }
    }
}
