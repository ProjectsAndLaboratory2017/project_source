using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplicationWPF.Model
{
    public class Product
    {
        public string ID { get; private set; }
        public string Barcode { get; private set; }
        public string Product_name { get; set; }
        public string Price { get; set; }
        public string Points { get; set; }
        public int StoreQty { get; set; }
        public int WarehouseQty { get; set; }
        public String Type { get; private set; }

        public Product(string id, string barcode, string name, double price, int points, int storeQty, int warehouseQty)
        {
            ID = id;
            Barcode = barcode;
            Product_name = name;
            Price = price.ToString("N2");
            Points = points.ToString();
            StoreQty = storeQty;
            this.WarehouseQty = warehouseQty;
            Type = "product";
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
            //return "{\"Type\":\"product\",\"ID\":\"" + ID + "\",\"Product_name\":\"" + Product_name + "\",\"Price\":\"" + Price + "\",\"Points\":\"" + Points + "\"}";
        }
    }
}
