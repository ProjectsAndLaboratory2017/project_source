using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplicationWPF.Model
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
