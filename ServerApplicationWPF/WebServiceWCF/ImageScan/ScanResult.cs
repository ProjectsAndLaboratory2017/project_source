using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServiceWCF.ImageScan
{
    public class ScanResult
    {
        public enum ResultType
        {
            None = 0,
            Barcode = 1,
            QR = 2
        }
        public ResultType Type { get; private set; }
        public String Value { get; private set; }
        public ScanResult(ResultType type, String value)
        {
            Type = type;
            Value = value;
        }

        public ScanResult(String ocr)
        {
            // TODO
        }
    }
}