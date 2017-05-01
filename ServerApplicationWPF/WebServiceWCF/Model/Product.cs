using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServiceWCF.Model
{
    public class Product
    {
        public string ProductId { get; private set; }
        public string Barcode { get; private set; }
        public string Name { get; private set; }
        public double Price { get; private set; }
        public int Points { get; private set; }

        public Product(string id, string barcode, string name, double price, int points)
        {
            ProductId = id;
            Barcode = barcode;
            Name = name;
            Price = price;
            Points = points;
        }

        public string Serialize()
        {
            // TODO use this method to serialize
            return "{\"id\": " + ProductId + ", \"name\": \"" + Name + "\", \"price\": " + Price + " }";
        }
    }
}