using MySql.Data.MySqlClient;
using WebServiceWCF.Model;
using System.Collections.Generic;

namespace WebServiceWCF.Database
{
    class DataManager
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        //Constructor
        public DataManager()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server = "localhost";
            database = "plcs";
            uid = "plcs";
            password = "plcs";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        throw new System.Exception("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        throw new System.Exception("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                throw new System.Exception(ex.Message);
                return false;
            }
        }

        //Insert statement
        public void InsertReceipt(Receipt receipt)
        {
            string query = "INSERT INTO receipt (ReceiptId, ProductId, CustomerId, Date, Quantity) VALUES(@receipt, @product, @customer, @date, @quantity)";

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);
                // prepare statement to avoid SQL injection
                cmd.Prepare();

                int receiptId = 123; // TODO get biggest receiptId and increment it

                MySqlParameter productId = new MySqlParameter("@product", "FILL_ME");
                MySqlParameter quantity = new MySqlParameter("@quantity", "FILL_ME");
                // set common values for all the items
                cmd.Parameters.AddWithValue("@receipt", receiptId);
                cmd.Parameters.Add(productId);
                cmd.Parameters.AddWithValue("@customer", receipt.CustomerId);
                cmd.Parameters.AddWithValue("@date", System.DateTime.Now.GetDateTimeFormats('d')[0]);
                cmd.Parameters.Add(quantity);
                foreach (var item in receipt.Items)
                {
                    productId.Value = item.Key;
                    quantity.Value = item.Value;

                    //Execute command
                    cmd.ExecuteNonQuery();
                }

                //close connection
                this.CloseConnection();
            }
        }

        //Select statement
        public Product getProductByBarcode(string barcode)
        {
            string query = "SELECT * FROM product where barcode=@text";

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //prepare the statement to avoid SQL injection
                cmd.Prepare();
                // put parameter
                cmd.Parameters.AddWithValue("@text", barcode);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                dataReader.Read();
                Product result = null;
                if (dataReader.HasRows)
                {
                    result = new Product(dataReader["ProductID"] + "", dataReader["Barcode"] + "", dataReader["Name"] + "", double.Parse(dataReader["Price"] + ""), int.Parse(dataReader["points"] + ""));
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return result;
            }
            else
            {
                return null;
            }
        }

        public Customer getCustomerByBarcode(string barcode)
        {
            string query = "SELECT * FROM customer where barcode=@text";

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //prepare the statement to avoid SQL injection
                cmd.Prepare();
                // put parameter
                cmd.Parameters.AddWithValue("@text", barcode);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                dataReader.Read();
                Customer result = null;
                if (dataReader.HasRows)
                {
                    result = new Customer(dataReader["CustomerID"] + "", dataReader["Barcode"] + "", dataReader["FirstName"] + "", dataReader["LastName"] + "", dataReader["Email"] + "");
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
