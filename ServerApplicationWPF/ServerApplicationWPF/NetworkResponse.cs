using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplicationWPF
{
    public class NetworkResponse
    {
        public byte[] Payload { get; private set; }
        public ResponseType responseType { get; private set; }
        public enum ResponseType
        {
            ImageProcessingResult = 2,
            ImageProcessingError = 3,
            ReceiptStorageResult = 4,
            ReceiptStorageError = 5
        }

        public NetworkResponse(ResponseType type, byte[] payload)
        {
            responseType = type;
            Payload = payload;
        }
    }
}
