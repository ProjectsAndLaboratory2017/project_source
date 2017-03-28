using System;
using System.Collections.Generic;
using System.Linq;
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
        public delegate object ImageReceivedCallbackType(object image);
        public delegate object ReceiptReceivedCallbackType(object receipt);

        private ImageReceivedCallbackType ImageReceivedCallback { get; set; }
        // this callback will be called when a receipt is read from the socket
        private ReceiptReceivedCallbackType ReceiptReceivedCallback { get; set; }
        public NetworkDriver(ImageReceivedCallbackType imageReceivedCallback, ReceiptReceivedCallbackType receiptReceivedCallback)
        {
            ImageReceivedCallback = imageReceivedCallback;
            ReceiptReceivedCallback = receiptReceivedCallback;
            SocketThread = new Thread(new ParameterizedThreadStart(start_listener));
            SocketThread.Start();
        }
        private void start_listener(object obj)
        {
            // TODO
            // listen on socket
            // wait requests
            // read request
            NetworkRequest request = readRequest();
            if (request.requestType == NetworkRequest.RequestType.ImageProcessingRequest)
            {
                // the board wants the server to process the image
                // call the callback
                object result = ImageReceivedCallback(request.Payload);
                // send the result to the board
            } else if (request.requestType == NetworkRequest.RequestType.ReceiptStorageRequest)
            {
                // the board wants the server to process the receipt
                // call the callback
                object result = ReceiptReceivedCallback(request.Payload);
                // send the result to the board
            }
            
        }

        private NetworkRequest readRequest()
        {
            // TODO read from network
            return new NetworkRequest();
        }

    }
}
