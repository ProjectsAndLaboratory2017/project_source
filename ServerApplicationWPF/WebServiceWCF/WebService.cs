using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using WebServiceWCF.ImageScan;

namespace WebServiceWCF
{
    public class WebService : IHttpHandler
    {
        private Database.DataManager dataManager;
        private ImageScan.CodeProjectWrapper codeScanner;
        public bool IsReusable { get { return false; } }

        public WebService()
        {
            dataManager = new Database.DataManager();
            codeScanner = new ImageScan.CodeProjectWrapper();
        }

        public void ProcessRequest(HttpContext context)
        {
            String path = context.Request.Path;
            context.Response.ContentType = "application/json";

            // figure out which HTTP method is being used
            switch (context.Request.HttpMethod)
            {
                // dispatch to internal methods based on URI and HTTP method
                // and write the correct response status & entity body
                case "GET":
                    if (path == "/users")
                    {
                        // TODO find a match with pathParam
                        context.Response.Write(new Model.Customer("1", "ciao", "ciao", "ciao", "ciao"));
                    }
                    else
                    {
                        HttpContext.Current.Response.Write("Hi there! The service is up and running");
                    }
                    break;
                case "POST":
                    if (path == "/barcode/recognize")
                    {
                        //HttpContext.Current.Response.Write("You want to recognize a image");
                        ImageScan.ScanResult scanResult = recognizeImage(context.Request.InputStream);
                        switch (scanResult.Type)
                        {
                            case ImageScan.ScanResult.ResultType.None:
                                // TODO
                                context.Response.Write("image without barcode");
                                break;
                            case ImageScan.ScanResult.ResultType.Barcode:
                                // TODO a product
                                Model.Product product = dataManager.getProductByBarcode(scanResult.Value);
                                if (product != null)
                                {
                                    context.Response.Write(product.Serialize());
                                }
                                else
                                {
                                    context.Response.StatusCode = 400;
                                    context.Response.Write("product not found");
                                }

                                break;
                            case ImageScan.ScanResult.ResultType.QR:
                                // TODO a customer
                                Model.Customer customer = dataManager.getCustomerByBarcode(scanResult.Value);
                                if (customer != null)
                                {
                                    context.Response.Write(customer.Serialize());
                                }
                                else
                                {
                                    context.Response.StatusCode = 400;
                                    context.Response.Write("customer not found");
                                }
                                break;
                            default:
                                context.Response.StatusCode = 400;
                                context.Response.Write("unexpected type of scan result");
                                break;
                        }
                    }
                    else if (path == "/receipts")
                    {
                        //Model.Receipt receipt = JsonConvert.DeserializeObject<Model.Receipt>();
                        /*HttpContext.Current.Response.Write("You want to store a receipt");
                        Model.Receipt receipt = parseReceipt(context);
                        dataManager.InsertReceipt(receipt);
                        // TODO check return value
                        */
                    }
                    break;
                default:
                    HttpContext.Current.Response.StatusCode = 405;
                    break;
            }
        }

        private ScanResult recognizeImage(Stream inputStream)
        {
            Bitmap bitmap = new Bitmap(inputStream);
            return codeScanner.ScanPage(bitmap);
        }
    }
}