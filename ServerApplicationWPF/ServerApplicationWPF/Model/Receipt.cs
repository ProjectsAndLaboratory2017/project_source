using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplicationWPF.Model
{

    public class Receipt
    {
        // the string that identifies the customer that produced the receipt
        public string CustomerId { get; private set; }
        // for each productId, store the quantity
        public Dictionary<string, int> Items { get; private set; }

        public Receipt(string customerId)
        {
            CustomerId = customerId;
            Items = new Dictionary<string, int>();
        }
    }
}
