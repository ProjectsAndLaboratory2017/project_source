using System;
using System.Net;
using System.Net.Sockets;

namespace ConsoleApplication_FakeClient.UDPNetwork
{
    public class Client:Peer
    {
        public Client(IPEndPoint serverEndpoint)
        {
            udpClient = new UdpClient();
            udpClient.Client.ReceiveTimeout = 10000;
            udpClient.Connect(serverEndpoint);
            remoteEndpoint = serverEndpoint;
        }

        public int AskToken()
        {
            byte[] req = System.Text.Encoding.Default.GetBytes("I want a token");
            TokenAndData reqTD = new TokenAndData(0, req);
            try
            {
                udpClient.Send(reqTD.Serialized, reqTD.Serialized.Length);
                byte[] res = udpClient.Receive(ref remoteEndpoint);
                TokenAndData resTD = new TokenAndData(res);
                if (resTD.Token != 0)
                {
                    throw new Exception("invalid token " + resTD.Token);
                }
                return BitConverter.ToInt32(resTD.Data, 0);
            }
            catch (System.Exception e)
            {
                throw new Exception("error asking for a token: " + e.Message);
            }
        }

    }
}
