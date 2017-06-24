using Newtonsoft.Json.Linq;
using ServerApplicationWPF.Model;
using ServerApplicationWPF.UDPNetwork;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplicationWPF
{
    public class ProductFinder
    {
        private String website = "https://api.upcdatabase.org/json/5ba5e61754b9801c09d20199216e6d31/";
        public Product search(String barcode)
        {
            try
            {
                HttpClient http = new HttpClient();
                var response = http.GetByteArrayAsync(website + barcode).Result;
                String source = Utils.BytesToString(response);
                var result = JObject.Parse(source);
                double price = 0;
                double.TryParse(result["avg_price"].ToString(), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out price);

                return new Product(null, barcode, result["description"].ToString(), price, 0, 0, 0);

            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
