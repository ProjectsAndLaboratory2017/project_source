using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace WebServiceWCF.ImageScan
{
    public class MessagingToolkitBarcodeWrapper
    {
        Dynamsoft.Barcode.BarcodeReader decoder;
        public MessagingToolkitBarcodeWrapper()
        {
            decoder = new Dynamsoft.Barcode.BarcodeReader();
        }

        public ScanResult ScanPage(Bitmap image)
        {
            string filePath = Path.GetTempPath() + "plcs\\" + System.DateTime.Now.Ticks + ".bmp";
            // save to temp file because ocr wants a file
            image.Save(filePath);
            var scanned = decoder.DecodeBitmap(image);
            string found = scanned[0].BarcodeText;
            Match match = Regex.Match(found, @"\[\[(.*): (.*)\]\]");
            string type = match.Groups[1].Value;
            string value = match.Groups[2].Value;
            if (type == "QR-Code")
            {
                return new ScanResult(ScanResult.ResultType.QR, value);
            }
            else if (type != null)
            {
                return new ScanResult(ScanResult.ResultType.Barcode, found);
            }
            else
            {
                return new ScanResult(ScanResult.ResultType.None, "ciaone");
            }
        }
    }
}