using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asprise_ocr_api;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace WebServiceWCF.ImageScan
{
    class CodeScanner
    {
        AspriseOCR ocr;
        public CodeScanner()
        {
            AspriseOCR.SetUp();
            ocr = new AspriseOCR();
            ocr.StartEngine("eng", AspriseOCR.SPEED_FASTEST);
        }

        public ScanResult ScanPage(Bitmap image)
        {
            string filePath = Path.GetTempPath() + "plcs\\" + System.DateTime.Now.Ticks + ".bmp";
            // save to temp file because ocr wants a file
            image.Save(filePath);
            string scanned = ocr.Recognize(filePath, -1, -1, -1, -1, -1, AspriseOCR.RECOGNIZE_TYPE_BARCODE, AspriseOCR.OUTPUT_FORMAT_PLAINTEXT);
            Match match = Regex.Match(scanned, @"\[\[(.*): (.*)\]\]");
            string type = match.Groups[1].Value;
            string value = match.Groups[2].Value;
            if (type == "QR-Code")
            {
                return new ScanResult(ScanResult.ResultType.QR, value);
            } else if (type != null)
            {
                return new ScanResult(ScanResult.ResultType.Barcode, value);
            }
            else
            {
                return new ScanResult(ScanResult.ResultType.None, scanned);
            }
            //[[EAN-13: 5901234123457]]\n
        }
    }
}
