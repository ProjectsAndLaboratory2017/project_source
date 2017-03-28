using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplicationWPF
{
    class NetworkRequest
    {
        public byte[] Payload { get; private set; }
        public RequestType requestType { get; private set; }
        public enum RequestType
        {
            ImageProcessingRequest = 0,
            ReceiptStorageRequest = 1
        }
    }
}
