using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace WebServiceWCF.ImageScan
{
    public class CodeProjectWrapper
    {

        public CodeProjectWrapper()
        {
        }

        public ScanResult ScanPage(Bitmap image)
        {
            string filePath = Path.GetTempPath() + "plcs\\" + System.DateTime.Now.Ticks + ".bmp";
            // save to temp file because ocr wants a file
            image.Save(filePath);
            ArrayList codes = new ArrayList();
            BarcodeScanner.FullScanPage(ref codes, image, 100);
            if (codes.Count > 0)
            {
                return new ScanResult(ScanResult.ResultType.Barcode, (string)codes[0]);
            }

            return new ScanResult(ScanResult.ResultType.None, "ciaone");
        }
    }
}