using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServiceWCF.Model
{
    public class Customer
    {
        public string CustomerId { get; private set; }
        public string Barcode { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Email { get; private set; }

        public Customer(string id, string barcode, string name, string surname, string email)
        {
            CustomerId = id;
            Barcode = barcode;
            Name = name;
            Surname = surname;
            Email = email;
        }

        public string Serialize()
        {
            return "{\"id\": " + CustomerId + ", \"name\": \"" + Name + "\", \"surname\": " + Surname + " }";
        }
    }
}