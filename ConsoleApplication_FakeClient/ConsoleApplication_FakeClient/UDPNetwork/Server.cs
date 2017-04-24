using System;
using System.Net;
using System.Net.Sockets;

namespace ConsoleApplication_FakeClient.UDPNetwork
{
    public class Server:Peer
    {
        private Random randomGenerator;
        bool valid;
        public Server(int port)
        {
            valid = false;
            udpClient = new UdpClient(port);
            randomGenerator = new Random(System.DateTime.Now.Millisecond);
        }

        /// <summary>
        /// accepts a token request
        /// </summary>
        /// <returns>the token that has been provided to the client</returns>
        public int Accept()
        {
            TokenAndData request_parsed;
            // set infinite timeout
            udpClient.Client.ReceiveTimeout = -1;
            do
            {
                remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] request = udpClient.Receive(ref remoteEndpoint);
                request_parsed = new TokenAndData(request);
            } while (request_parsed.Token != 0); // TODO could also check data "I want a token"
            // connect to this specific client
            udpClient.Connect(remoteEndpoint);

            int token = randomGenerator.Next();

            TokenAndData response_token = new TokenAndData(0, BitConverter.GetBytes(token));
            udpClient.Send(response_token.Serialized, response_token.Serialized.Length);

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
