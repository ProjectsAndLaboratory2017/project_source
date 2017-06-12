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
using ServerApplicationWPF.Model;
using ZXing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerApplicationWPF.UDPNetwork;

namespace ServerApplicationWPF
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NetworkDriver networkDriver;
        private CodeScanner codeScanner;
        private DataManager dbConnect;

        private delegate void StringConsumer(string s);
        private delegate void ImageConsumer(Bitmap image);
        public MainWindow()
        {
            InitializeComponent();
            codeScanner = new CodeScanner();
            networkDriver = new NetworkDriver(requestProcessing, messageProcessing);
            dbConnect = new DataManager();
            Console.WriteLine("ciao");
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
                // do the barcode scan
                ArrayList barcodes = new ArrayList();
                //BarcodeScanner.FullScanPage(ref barcodes, image, 100);
                IBarcodeReader reader = new BarcodeReader();
                var result = reader.Decode(image);

                //string result = codeScanner.ScanPage(image);
                int rotation = 0;
                while (result == null && rotation < 36)
                {
                    //image = rotateImage90(image);
                    var image_tmp = RotateImg(image, rotation * 10, System.Drawing.Color.Transparent);
                    result = reader.Decode(image_tmp);
                    rotation++;
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new ImageConsumer(displayImage), image_tmp);
                }
                //messageProcessing("Scan done. Found " + barcodes.Count + "barcodes");
                NetworkResponse response;
                if (result != null)
                {
                    if (result.BarcodeFormat == BarcodeFormat.QR_CODE)
                    {
                        // this is a user
                        string textResult = result.Text;
                        messageProcessing("Scan done. Found QR code: " + result);
                        Customer c = dbConnect.getCustomerByBarcode(textResult);
                        if (c == null)
                        {
                            messageProcessing("No user found with this barcode");
                            response = new NetworkResponse(NetworkResponse.ResponseType.ImageProcessingError, Utils.StringToBytes("Error"));
                        }
                        else
                        {
                            messageProcessing("User found: " + c.Email);
                            response = new NetworkResponse(NetworkResponse.ResponseType.ImageProcessingResult, Utils.StringToBytes(c.ToString()));
                        }
                    }
                    else
                    {
                        // this should be a product
                        string textResult = result.Text;
                        messageProcessing("Scan done. Found: " + result);
                        Product p = dbConnect.getProductByBarcode(textResult);
                        if (p == null)
                        {
                            messageProcessing("No products found");
                            response = new NetworkResponse(NetworkResponse.ResponseType.ImageProcessingError, Utils.StringToBytes("Error"));
                        }
                        else if (p.StoreQty < 1)
                        {
                            messageProcessing("Product quantity less than 1");
                            response = new NetworkResponse(NetworkResponse.ResponseType.ImageProcessingError, Utils.StringToBytes("Error"));
                        }
                        else
                        {
                            messageProcessing("Product found: " + p.Product_name);
                            response = new NetworkResponse(NetworkResponse.ResponseType.ImageProcessingResult, Utils.StringToBytes(p.ToString()));
                        }
                    }
                }
                else
                {
                    messageProcessing("No barcodes found");
                    //response = new NetworkResponse(NetworkResponse.ResponseType.ImageProcessingResult, Encoding.UTF8.GetBytes("{\"ID\":\"10\",\"Name\":\"Martino\",\"Surname\":\"Mensio\",\"Email\":\"martino@gmail.com\"}"));
                    response = new NetworkResponse(NetworkResponse.ResponseType.ImageProcessingError, Utils.StringToBytes("Error"));
                }
                return response;
            }
            else if (request.requestType == NetworkRequest.RequestType.ReceiptStorageRequest)
            {
                try
                {
                    string req = UDPNetwork.Utils.BytesToString(request.Payload);
                    JObject receipt = JObject.Parse(req);
                    // get customerId
                    String userId = receipt["UserID"].ToString();
                    JArray list = receipt["List"] as JArray;
                    IList<JToken> products = list.Children().ToList();
                    Receipt receiptObj = new Receipt(userId);

                    foreach (var product in products)
                    {
                        String id = product["ID"].ToString();
                        String qty = product["Qty"].ToString();

                        messageProcessing("product id: " + id + " qty: " + qty);

                        receiptObj.Items.Add(id, int.Parse(qty));
                    }

                    dbConnect.InsertReceipt(receiptObj);
                    // return ok to the board
                    return new NetworkResponse(NetworkResponse.ResponseType.ReceiptStorageResult, Utils.StringToBytes("OK"));
                }
                catch (Exception e)
                {
                    // some exception
                    Console.WriteLine("Exception catched reading a receipt: " + e.Message);
                    return new NetworkResponse(NetworkResponse.ResponseType.ReceiptStorageError, Utils.StringToBytes("Error"));
                }
            }
            else
            {
                // some errors
                Console.WriteLine("Unknown request type");
                return new NetworkResponse(NetworkResponse.ResponseType.ReceiptStorageError, Utils.StringToBytes("Error"));
            }
        }

        private void displayImage(Bitmap image)
        {
            //addMessageToLog("Trying to display image");
            //ImageDisplay.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(image);
            ImageDisplay.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private void addMessageToLog(string message)
        {
            try
            {
                Log.Text += "\n" + message;
                MyScrollViewer.ScrollToBottom();
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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Product product = dbConnect.getProductByBarcode(barcode_txt.Text);
            db_output.Text = product.Product_name;
        }

        private Bitmap rotateImage90(Bitmap b)
        {
            Bitmap returnBitmap = new Bitmap(b.Height, b.Width);
            Graphics g = Graphics.FromImage(returnBitmap);
            g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
            g.RotateTransform(90);
            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
            g.DrawImage(b, new System.Drawing.Point(0, 0));
            return returnBitmap;
        }

        private static Bitmap RotateImg(Bitmap bmp, float angle, System.Drawing.Color bkColor)
        {
            angle = angle % 360;
            if (angle > 180)
                angle -= 360;

            System.Drawing.Imaging.PixelFormat pf = default(System.Drawing.Imaging.PixelFormat);
            if (bkColor == System.Drawing.Color.Transparent)
            {
                pf = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            }
            else
            {
                pf = bmp.PixelFormat;
            }

            float sin = (float)Math.Abs(Math.Sin(angle * Math.PI / 180.0)); // this function takes radians
            float cos = (float)Math.Abs(Math.Cos(angle * Math.PI / 180.0)); // this one too
            float newImgWidth = sin * bmp.Height + cos * bmp.Width;
            float newImgHeight = sin * bmp.Width + cos * bmp.Height;
            float originX = 0f;
            float originY = 0f;

            if (angle > 0)
            {
                if (angle <= 90)
                    originX = sin * bmp.Height;
                else
                {
                    originX = newImgWidth;
                    originY = newImgHeight - sin * bmp.Width;
                }
            }
            else
            {
                if (angle >= -90)
                    originY = sin * bmp.Width;
                else
                {
                    originX = newImgWidth - sin * bmp.Height;
                    originY = newImgHeight;
                }
            }

            Bitmap newImg = new Bitmap((int)newImgWidth, (int)newImgHeight, pf);
            Graphics g = Graphics.FromImage(newImg);
            g.Clear(bkColor);
            g.TranslateTransform(originX, originY); // offset the origin to our calculated values
            g.RotateTransform(angle); // set up rotate
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(bmp, 0, 0); // draw the image at 0, 0
            g.Dispose();

            return newImg;
        }
    }
}
