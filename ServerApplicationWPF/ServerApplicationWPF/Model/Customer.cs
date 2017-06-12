using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplicationWPF.Model
{
    
    public class Customer
    {
        public string ID { get; private set; }
        public string Barcode { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Email { get; private set; }
        public string Type { get; private set; }

        public Customer(string id, string barcode, string name, string surname, string email)
        {
            ID = id;
            Barcode = barcode;
            Name = name;
            Surname = surname;
            Email = email;
            Type = "user";
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
            //return "{\"Type\":\"customer\",\"ID\":\"" + CustomerId + "\",\"Name\":\"" + Name + "\",\"Surname\":\"" + Surname + "\",\"Email\":\"" + Email + "\"}";
        }
    }
}
