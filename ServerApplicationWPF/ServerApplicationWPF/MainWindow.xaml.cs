using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections;

namespace ServerApplicationWPF
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NetworkDriver networkDriver;

        private delegate void StringConsumer(string s);
        private delegate void ImageConsumer(Bitmap image);
        public MainWindow()
        {
            InitializeComponent();
            networkDriver = new NetworkDriver(requestProcessing, messageProcessing);
        }

        private void messageProcessing(string message)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new StringConsumer(addMessageToLog), message);
        }

        private NetworkResponse requestProcessing(NetworkRequest request)
        {
            if (request.requestType == NetworkRequest.RequestType.ImageProcessingRequest)
            {
                // create bitmap from array of bytes
                Bitmap image = new Bitmap(new MemoryStream(request.Payload));
                Bitmap image2 = new Bitmap(image);
                // show the image
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new ImageConsumer(displayImage), image2);
                // an array of strings
                ArrayList barcodes = new System.Collections.ArrayList();
                // do the barcode scan
                BarcodeScanner.FullScanPage(ref barcodes, image, 100);
                messageProcessing("Scan done. Found " + barcodes.Count + "barcodes");
                NetworkResponse response;
                if (barcodes.Count == 1)
                {
                    response = new NetworkResponse(NetworkResponse.ResponseType.ImageProcessingResult, Encoding.ASCII.GetBytes(barcodes[0].ToString()));
                }
                else
                {
                    response = new NetworkResponse(NetworkResponse.ResponseType.ImageProcessingError, new byte[0]);
                }
                return response;
            }
            else if (request.requestType == NetworkRequest.RequestType.ReceiptStorageRequest)
            {
                // TODO
                return null;
            } else
            {
                // some errors
                return null;
            }
        }

        private void displayImage(Bitmap image)
        {
            addMessageToLog("Trying to display image");
            //ImageDisplay.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(image);
            ImageDisplay.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private void addMessageToLog(string message)
        {
            try
            {
                Log.Text += "\n" + message;
            }
            catch (Exception e)
            {
                Debug.Print("Exception: " + e.Message);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //Application.Current.Shutdown(); not necessary because other thread is background
        }
    }
}
