using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace WebServiceWCF.ImageScan
{
    public class SpireScan
    {
        public SpireScan()
        {
            
        }

        public ScanResult ScanPage(Bitmap image)
        {
            string filePath = Path.GetTempPath() + "plcs\\" + System.DateTime.Now.Ticks + ".bmp";
            // save to temp file because ocr wants a file
            image.Save(filePath);
            string scanned = Spire.Barcode.BarcodeScanner.ScanOne(filePath);
            Match match = Regex.Match(scanned, @"\[\[(.*): (.*)\]\]");
            string type = match.Groups[1].Value;
            string value = match.Groups[2].Value;
            if (type == "QR-Code")
            {
                return new ScanResult(ScanResult.ResultType.QR, value);
            }
            else if (type != null)
            {
                return new ScanResult(ScanResult.ResultType.Barcode, value);
            }
            else
            {
                return new ScanResult(ScanResult.ResultType.None, scanned);
            }
            
        }
    }
}