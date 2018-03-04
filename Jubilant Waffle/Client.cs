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

        Client() {
            udp = new System.Net.Sockets.UdpClient();
            tcp = new System.Net.Sockets.TcpClient();
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
                            /* Add only if user is not already present */
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
            tcp.Connect(new System.Net.IPEndPoint(userAddress, 20000));
            throw new NotImplementedException();
        }
    }
}
