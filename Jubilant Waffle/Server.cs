using System;
namespace Jubilant_Waffle {
    class Server {

        #region UDPClient
        /* The UDP client is only in charge of sending periodically an announcement if the the status is online */
        System.Net.Sockets.UdpClient udp; //The client
        System.Timers.Timer announceTimer; //The timer that will execute periodically the announcement
        #endregion
        #region TCPSever
        System.Net.Sockets.TcpListener tcp;
        #endregion

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

        Server(int port) {
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
            tcp.Server.ReceiveTimeout = tcp.Server.SendTimeout = 2000; // The send timeout set the maxiumum amount of time that the Listener will wait befor throwing and exception
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
            while (_status) {
                #region Wait for new connections
                try {
                    System.Net.Sockets.TcpClient incomingConnection = tcp.AcceptTcpClient();
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
                new System.Threading.Thread(() => ManageConnection());
                #endregion
            }
        }

        private void ManageConnection() {
            throw new NotImplementedException();
        }
    }
}