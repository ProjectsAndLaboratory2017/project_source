using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServiceWCF.Model
{
    public class Receipt
    {
        // TODO consider Customer
        public string CustomerId { get; private set; }
        // TODO consider Dictionary<Product, int>
        public Dictionary<string, int> Items { get; private set; }

        public Receipt(string customerId)
        {
            CustomerId = customerId;
            Items = new Dictionary<string, int>();
        }
    }
}