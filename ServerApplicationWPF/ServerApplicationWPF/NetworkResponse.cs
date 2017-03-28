using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplicationWPF
{
    class NetworkResponse
    {
        public byte[] Payload { get; private set; }
        public ResponseType responseType { get; private set; }
        public enum ResponseType
        {
            ImageProcessingResult = 2,
            ReceiptStorageResult = 3
        }
    }
}
