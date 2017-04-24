using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asprise_ocr_api;
using System.Drawing;
using System.IO;

namespace ServerApplicationWPF
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

        public string ScanPage(Bitmap image)
        {
            string filePath = Path.GetTempFileName() + ".bmp";
            // save to temp file because ocr wants a file
            image.Save(filePath);
            string result = ocr.Recognize(filePath, -1, -1, -1, -1, -1, AspriseOCR.RECOGNIZE_TYPE_BARCODE, AspriseOCR.OUTPUT_FORMAT_PLAINTEXT);
            return result;
        }
    }
}
