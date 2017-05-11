using System;
using System.Net;
using System.Net.Sockets;

namespace ConsoleApplication_FakeClient.UDPNetwork
{
    public class Client : Peer
    {
        public Client(IPEndPoint serverEndpoint)
        {
            socket.ReceiveTimeout = Utils.RECEIVE_TIMEOUT;
            socket.Connect(serverEndpoint);
            remoteEndpoint = serverEndpoint;
        }

        public int AskToken()
        {
            byte[] req = Utils.StringToBytes("I want a token");
            TokenAndData reqTD = new TokenAndData(0, 0, req);
            try
            {
                socket.Send(reqTD.Serialized);
                byte[] res = Utils.Receive(socket);
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
