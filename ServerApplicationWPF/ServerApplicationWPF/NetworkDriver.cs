using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerApplicationWPF
{
    public class NetworkDriver
    {
        public Thread SocketThread { get; private set; }
        // TODO change the object type to the most appropriate image type
        // TODO the return type is what needs to be sent back to the board
        public delegate NetworkResponse RequestToResponseCallbackType(NetworkRequest request);
        public delegate void MessageCallbackType(string message);

        private RequestToResponseCallbackType requestCallback;
        private MessageCallbackType messageCallback;
        public NetworkDriver(RequestToResponseCallbackType requestCallback, MessageCallbackType messageCallback)
        {
            this.requestCallback = requestCallback;
            this.messageCallback = messageCallback;
            SocketThread = new Thread(new ParameterizedThreadStart(start_listener));
            SocketThread.Start();
        }
        private void start_listener(object obj)
        {
            // TODO
            // listen on socket
            // wait requests
            // read request
            //create server socket bind and start listen
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 255);
            IPEndPoint endP = new IPEndPoint(IPAddress.Any, 8000);
            listenSocket.Bind(endP);
            listenSocket.Listen(10);

            //accept a client
            while (true)
            {
                messageCallback("Waiting connections");
                Socket acceptedSocket = listenSocket.Accept();

                //print to screen message when socket is accepted
                messageCallback("Accepted connection from " + acceptedSocket.RemoteEndPoint);
                while (true)
                {
                    NetworkRequest request = readRequest(acceptedSocket);
                    if (request != null)
                    {
                        messageCallback("Received a request of type " + request.requestType);
                        // call the callback
                        NetworkResponse response = requestCallback(request);
                        messageCallback("Sending a response of type " + response.responseType);
                        if (sendResponse(acceptedSocket, response))
                        {
                            // success
                            messageCallback("Sent successfully");
                        }
                        else
                        {
                            // error sending response
                            messageCallback("Error sending response");
                        }
                    }
                    else
                    {
                        // error receiving request
                        messageCallback("Error receiving request");
                    }
                    // TODO if socket closed, exit loop
                }
            }



        }

        private NetworkRequest readRequest(Socket s)
        {
            // TODO read from network
            NetworkRequest.RequestType type;
            byte[] payload;

            Int32 bytesRead = 0;
            int count;
            int i = 0;
            int payloadLength;

            byte[] buffer;

            try
            {
                // read the request type
                buffer = new Byte[sizeof(int)];
                count = sizeof(int);
                while (count > 0)
                {
                    bytesRead = s.Receive(buffer, i, count, SocketFlags.None);
                    i += bytesRead;
                    count -= bytesRead;
                    // TODO detect when connection was closed (bytesRead == 0)
                }
                type = (NetworkRequest.RequestType)BitConverter.ToInt32(buffer, 0);

                // read the 32 bit length
                buffer = new Byte[sizeof(int)];
                count = sizeof(int);
                i = 0;
                payloadLength = sizeof(int);
                while (count > 0)
                {
                    bytesRead = s.Receive(buffer, i, count, SocketFlags.None);
                    i += bytesRead;
                    count -= bytesRead;
                }
                payloadLength = BitConverter.ToInt32(buffer, 0);

                // read the payload
                payload = new Byte[payloadLength];
                count = payload.Length;
                i = 0;
                bytesRead = 0;
                while (count > 0)
                {
                    try
                    {
                        bytesRead = s.Receive(payload, i, count, SocketFlags.None);
                        i += bytesRead;
                        count -= bytesRead;
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.WouldBlock ||
                            ex.SocketErrorCode == SocketError.IOPending ||
                            ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                        {
                            // socket buffer is probably empty, wait and try again
                            Thread.Sleep(30);
                        }
                        else
                            throw ex;  // any serious error occurr
                    }
                }

                //return data;
            }
            catch (Exception e)
            {
                messageCallback("Exception in receiving request: " + e.Message);
                return null;
            }
            return new NetworkRequest(type, payload);
        }

        private bool sendResponse(Socket s, NetworkResponse response)
        {
            try
            {
                // send responseType
                s.Send(BitConverter.GetBytes((Int32)response.responseType), sizeof(Int32), 1, SocketFlags.None);
                // send response payload length
                s.Send(BitConverter.GetBytes(response.Payload.Length), sizeof(Int32), 1, SocketFlags.None);
                // send payload
                int count = response.Payload.Length;
                int i = 0, byteInviati;
                Byte[] sData = response.Payload;
                while (count > 0)
                {
                    byteInviati = s.Send(sData, i, count, SocketFlags.None);
                    count -= byteInviati;
                    i += byteInviati;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

    }
}
