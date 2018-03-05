using System;
namespace Jubilant_Waffle {
    class Server {
        User self; // Represent the user running the application
        #region UDPClient
        /* The UDP client is only in charge of sending periodically an announcement if the the status is online */
        System.Net.Sockets.UdpClient udp; //The client
        System.Timers.Timer announceTimer; //The timer that will execute periodically the announcement
        #endregion
        #region TCPSever
        System.Net.Sockets.TcpListener tcp;
        #endregion

        bool _cancelCurrent = false; // This is used to undo the current transfer
        bool _status = false; //The status
        bool Status {
            get {
                return _status;
            }
            set {
                /* If the user set true and server is not running, start both services */
                if (value && !_status) {
                    tcp.Start();
                    announceTimer.Enabled = true;
                }
                /* If the user set false and server is running, stop both services */
                if (!value && _status) {
                    tcp.Stop();
                    announceTimer.Enabled = false;
                }
            }
        }

        public Server(int port, string name, string image = null) {
            #region Setup personal info
            self = new User(name, "127.0.0.1", image);
            #endregion
            #region UDP Client/Server and Timer setup
            udp = new System.Net.Sockets.UdpClient();
            udp.Connect(System.Net.IPAddress.Broadcast, port); //Set the default IP address. It is the broadcast address and the port passed to the constructor

            /* 
             * Setup the timer for the announcement
             */
            announceTimer = new System.Timers.Timer();
            announceTimer.Elapsed += Announce; // The callback that will execute the announcement has to be added to the event Elapsed
            announceTimer.Interval = 2000; // The timeout interval
            announceTimer.AutoReset = true; // Make the execution repeat several times
            #endregion 
            #region TCP Server setup
            tcp = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, port);
            tcp.Server.ReceiveTimeout = tcp.Server.SendTimeout = 2000; // The timeout set the maxiumum amount of time that the Listener will wait befor throwing and exception
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
                catch (System.Net.Sockets.SocketException e) {
                    /* 
                     * SocketException is launched at timeout elased. Nothing as to be done, 
                     * but thanks to the timeout the status condition is periodically checked.
                     */
                    continue;
                }
                #endregion
                #region new incoming connection
                new System.Threading.Thread(() => ManageConnection(incomingConnection));
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
            catch (System.Net.Sockets.SocketException e) {
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
            if (self.imagePath != null)
                data = System.Text.Encoding.ASCII.GetBytes("MARIO");
            else
                data = System.Text.Encoding.ASCII.GetBytes("LUIGI");
            try {
                client.GetStream().Write(data, 0, data.Length);
            }
            catch (System.Net.Sockets.SocketException e) {
                /* Could not connect to the host, something went wrong. Request aborted */
                System.Console.Write("Impossible serving personal info request, failed sending response");
                return;
            }
            #endregion
            #region Send name lenght
            data = System.BitConverter.GetBytes(self.name.Length);
            try {
                client.GetStream().Write(data, 0, data.Length);
            }
            catch (System.Net.Sockets.SocketException e) {
                /* Could not connect to the host, something went wrong. Request aborted */
                System.Console.Write("Impossible serving personal info request, failed sending name lenght");
                return;
            }
            #endregion
            #region Send name
            data = System.Text.Encoding.ASCII.GetBytes(self.name);
            try {
                client.GetStream().Write(data, 0, data.Length);
            }
            catch (System.Net.Sockets.SocketException e) {
                /* Could not connect to the host, something went wrong. Request aborted */
                System.Console.Write("Impossible serving personal info request, failed sending name");
                return;
            }
            #endregion
            if (self.imagePath != null) {
                #region Send image lenght
                imageLenght = (new System.IO.FileInfo(self.imagePath)).Length;
                data = System.BitConverter.GetBytes(imageLenght);
                try {
                    client.GetStream().Write(data, 0, data.Length);
                }
                catch (System.Net.Sockets.SocketException e) {
                    /* Could not connect to the host, something went wrong. Request aborted */
                    System.Console.Write("Impossible serving personal info request, failed sending image lenght");
                    return;
                }
                #endregion
                #region Send Image
                client.Client.SendFile(self.imagePath);
                #endregion
            }
        }

        private void ReceiveFile(System.Net.Sockets.TcpClient client) {
            byte[] data;
            int fileNameLenght;
            string filename;
            long fileSize;
            long alreadyReceived = 0;
            System.IO.FileStream fs;
            #region Read file name lenght
            data = new byte[4];
            try {
                client.GetStream().Read(data, 0, 4);
            }
            catch (System.Net.Sockets.SocketException e) {
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
            catch (System.Net.Sockets.SocketException e) {
                /* Could not connect to the host, something went wrong. Request aborted */
                System.Console.Write("Impossible receiving file, failed reading file name");
                return;
            }
            filename = System.Text.Encoding.ASCII.GetString(data);
            #endregion
            #region Read file size
            data = new byte[8];
            try {
                client.GetStream().Read(data, 0, 8);
            }
            catch (System.Net.Sockets.SocketException e) {
                /* Could not connect to the host, something went wrong. Request aborted */
                System.Console.Write("Impossible receiving file, failed reading file size");
                return;
            }
            fileSize = System.BitConverter.ToInt64(data, 0);
            #endregion
            #region Receive file
            data = new byte[4 * 1024 * 1024];
            //TODO existing file will be automatically overwritten. Modify this behaviour later
            fs = new System.IO.FileStream(filename, System.IO.FileMode.Create);
            while (alreadyReceived < fileSize && !_cancelCurrent) {
                try {
                    client.GetStream().Read(data, 0, (int)System.Math.Min((long)4 * 1024 * 1024, fileSize - alreadyReceived));
                }
                catch (System.Net.Sockets.SocketException e) {
                    /* Could not connect to the host, something went wrong. Request aborted */
                    System.Console.Write("Impossible receiving file, failed reading file size");
                    return;
                }

            }
            /* reset cancelCurrent. It assures that if it has been sent, it wont be active for next file in the list */
            _cancelCurrent = false;
            #endregion
            throw new NotImplementedException();
        }
    }
}