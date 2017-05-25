using Newtonsoft.Json;
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
            // background threads don't keep app running if other threads terminate
            SocketThread.IsBackground = true;
            SocketThread.Start();
        }
        private void start_listener(object obj)
        {
            // set the dot separator for floats
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            UDPNetwork.Server server = new UDPNetwork.Server(8000);

            //accept a client
            while (true)
            {
                try
                {
                    messageCallback("Waiting connections");
                    int token = server.Accept();
                    // TODO get details from accepted connection
                    messageCallback("Accepted connection. Gave token: " + token);

                    NetworkRequest request = readRequest(server, token);
                    if (request != null)
                    {
                        messageCallback("Received a request of type " + request.requestType);
                        // call the callback
                        NetworkResponse response = requestCallback(request);
                        messageCallback("Sending a response of type " + response.responseType);
                        if (sendResponse(server, response, token))
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
                }
                catch (System.Exception e)
                {
                    messageCallback("Exception occurred: " + e.Message);
                }
            }



        }

        private NetworkRequest readRequest(UDPNetwork.Server server, int token)
        {
            // TODO read from network
            NetworkRequest.RequestType type;
            byte[] payload;

            try
            {
                payload = server.ReceiveData(token);
                string req = UDPNetwork.Utils.BytesToString(payload);
                try
                {
                    JsonConvert.DeserializeObject(req);
                    type = NetworkRequest.RequestType.ReceiptStorageRequest;
                }
                catch (JsonException e)
                {
                    type = NetworkRequest.RequestType.ImageProcessingRequest;
                }
                
            }
            catch (Exception e)
            {
                messageCallback("Exception in receiving request: " + e.Message);
                return null;
            }
            return new NetworkRequest(type, payload);
        }

        private bool sendResponse(UDPNetwork.Server server, NetworkResponse response, int token)
        {
            try
            {
                // TODO for now only sending payload
                byte[] payload = response.Payload;

                server.SendData(payload, token);

                return true;
            }
            catch (Exception e)
            {
                messageCallback("Exception in sending response: " + e.Message);
                return false;
            }
        }

    }
}
