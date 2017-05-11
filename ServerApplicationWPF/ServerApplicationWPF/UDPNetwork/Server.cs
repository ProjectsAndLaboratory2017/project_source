using System;
using System.Net;
using System.Net.Sockets;

namespace ServerApplicationWPF.UDPNetwork
{
    public class Server : Peer
    {
        private Random randomGenerator;
        bool valid;
        private int port;
        public Server(int port)
        {
            valid = false;
            this.port = port;
            IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Any, port);
            socket.Bind(localEndpoint);
            randomGenerator = new Random(System.DateTime.Now.Millisecond);
        }

        /// <summary>
        /// accepts a token request
        /// </summary>
        /// <returns>the token that has been provided to the client</returns>
        public int Accept()
        {
            TokenAndData request_parsed;

            socket.ReceiveTimeout = -1;
            do
            {
                remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] datagram = Utils.ReceiveFrom(socket, ref remoteEndpoint);
                request_parsed = new TokenAndData(datagram);
            } while (request_parsed.Token != 0); // TODO could also check data "I want a token"
            // connect to this specific client
            //socket.Connect(remoteEndpoint);

            int token = randomGenerator.Next();

            TokenAndData response_token = new TokenAndData(0, 0, BitConverter.GetBytes(token));
            socket.SendTo(response_token.Serialized, remoteEndpoint);

            valid = true;
            return token;
        }

        public new void SendData(byte[] data, int token)
        {
            if (!valid)
            {
                throw new Exception("Accept a connection first, then call again SendData");
            }
            base.SendData(data, token);
        }

        public new byte[] ReceiveData(int token)
        {
            if (!valid)
            {
                throw new Exception("Accept a connection first, then call again ReceiveData");
            }
            return base.ReceiveData(token);
        }
    }
}
