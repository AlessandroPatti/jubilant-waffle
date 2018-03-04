using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jubilant_Waffle {
    class Client {
        /* Map of the users known. It indexed by IP address since each connection brings the IP */
        System.Collections.Generic.Dictionary<String, User> users;

        System.Net.Sockets.UdpClient udp;
        System.Net.Sockets.TcpClient tcp;

        public Client() {
            udp = new System.Net.Sockets.UdpClient();
            tcp = new System.Net.Sockets.TcpClient();
            tcp.ReceiveTimeout = tcp.SendTimeout = 2000; // The timeout set the maxiumum amount of time that the Listener will wait befor throwing and exception
        }

        public void SendFile() {

        }

        private void ClientRoutine() {
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
             *      - ITSME  when the message contains the image. In this case 
             *          the message will have this format
             *              - 4 byte of name lenght
             *              - name
             *              - 4 byte for image lengt
             *              - image
             *      - ME!!! when the user do not have any image. In this case 
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
            if (msg == "ITSME") {
                /* 
                 * First read the leght of the image file 
                 */
                Int32 imageLenght;
                byte[] image;
                try {
                    tcp.GetStream().Read(lenght, 0, 4);
                }
                catch (System.Net.Sockets.SocketException e) {
                    /* Could not connect to the host, something went wrong. Nothing will happen */
                    System.Console.Write("Impossible add new user, image lenght unsuccessful");
                    return;
                }
                imageLenght = System.BitConverter.ToInt32(lenght, 0); //TODO how to manage big and little endian?

                /* 
                 * Read the actual image file 
                 */
                image = new byte[imageLenght];
                try {
                    tcp.GetStream().Read(image, 0, imageLenght);
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
            if (msg == "ITSME") {
                users.Add(userAddress.ToString(), new User(name, userAddress.ToString(), "user_pic/" + userAddress.ToString() + ".png"));
            }
            else {
                users.Add(userAddress.ToString(), new User(name, userAddress.ToString()));
            }
            #endregion
        }
    }
}
