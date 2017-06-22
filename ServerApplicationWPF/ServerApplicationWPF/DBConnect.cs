using MySql.Data.MySqlClient;
using ServerApplicationWPF.Model;
using System.Collections.Generic;

public class DataManager
{
    private MySqlConnection connection;
    private string server;
    private string database;
    private string uid;
    private string password;

    //Constructor
    public DataManager()
    {
        server = "localhost";
        database = "plcs";
        uid = "plcs";
        password = "plcs";
        string connectionString;
        connectionString = "SERVER=" + server + ";" + "DATABASE=" +
        database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

        connection = new MySqlConnection(connectionString);

        OpenConnection();
    }

    ~DataManager()
    {
        connection.Close();
    }

    // TODO use transactions
    private MySqlTransaction getTransaction()
    {
        return connection.BeginTransaction();
    }
    //open connection to database
    private void OpenConnection()
    {
        try
        {
            connection.Open();
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

                case 1045:
                    throw new System.Exception("Invalid username/password, please try again");
                default:
                    throw new System.Exception("Exception opening connection to db: " + ex.Message);
            }
        }
    }

    //Close connection
    private void CloseConnection()
    {
        try
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                return;
            }
            connection.Close();
        }
        catch (MySqlException ex)
        {
            throw new System.Exception("Exception closing connection to database: " + ex.Message);
        }
    }

    void checkOpenConnection()
    {
        if (connection.State != System.Data.ConnectionState.Open)
        {
            CloseConnection();
            OpenConnection();
        }
    }

    //Insert statement
    public void InsertReceipt(Receipt receipt)
    {
        string query = "INSERT INTO receipt (ReceiptId, ProductId, CustomerId, Date, Quantity) VALUES(@receipt, @product, @customer, @date, @quantity)";
        string getMaxIdQuery = "SELECT max(ReceiptId) FROM receipt";
        //open connection
        checkOpenConnection();
        using (var transaction = connection.BeginTransaction())
        {

            /**
             * TODOs:
             * - decrease the quantity on the product table
             * - evaluation of the points and add points to the customer
             * 
             * 
             */
            // get the max receipt id
            MySqlCommand getMaxReceiptIdCmd = new MySqlCommand(getMaxIdQuery, connection, transaction);
            MySqlDataReader dataReader = getMaxReceiptIdCmd.ExecuteReader();
            dataReader.Read();
            int receiptId = 0;
            if (dataReader.HasRows)
            {
                receiptId = dataReader.GetInt32(0);
            }
            dataReader.Close();
            receiptId++;
            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(query, connection, transaction);
            // prepare statement to avoid SQL injection
            cmd.Prepare();


            MySqlParameter productId = new MySqlParameter("@product", "FILL_ME");
            MySqlParameter quantity = new MySqlParameter("@quantity", "FILL_ME");
            // set common values for all the items
            cmd.Parameters.AddWithValue("@receipt", receiptId);
            cmd.Parameters.Add(productId);
            cmd.Parameters.AddWithValue("@customer", receipt.CustomerId);
            cmd.Parameters.AddWithValue("@date", System.DateTime.Now.ToString("yyyy-MM-dd"));
            cmd.Parameters.Add(quantity);
            foreach (var item in receipt.Items)
            {
                productId.Value = item.Key;
                quantity.Value = item.Value;

                //Execute command
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
    }

    //Select statement
    public Product getProductByBarcode(string barcode)
    {
        string query = "SELECT * FROM product where barcode=@text";

        //Open connection
        checkOpenConnection();
        using (var transaction = connection.BeginTransaction())
        {
            //Create Command
            MySqlCommand cmd = new MySqlCommand(query, connection, transaction);
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
                result = new Product(dataReader["ProductID"] + "", dataReader["Barcode"] + "", dataReader["Name"] + "", double.Parse(dataReader["Price"] + ""), int.Parse(dataReader["Points"] + ""), int.Parse(dataReader["StoreQty"].ToString()), int.Parse(dataReader["WarehouseQty"].ToString()));
            }

            //close Data Reader
            dataReader.Close();

            transaction.Commit();

            //return list to be displayed
            return result;
        }
    }

    public Customer getCustomerByBarcode(string barcode)
    {
        string query = "SELECT * FROM customer where barcode=@text";

        //Open connection
        checkOpenConnection();
        using (var transaction = connection.BeginTransaction())
        {
            //Create Command
            MySqlCommand cmd = new MySqlCommand(query, connection, transaction);
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
            transaction.Commit();

            //return list to be displayed
            return result;
        }
    }

    public void insertProduct(Product product)
    {
        string query = "INSERT INTO product (ProductId, Barcode, Name, Price, StoreQty, WarehouseQty, Points) VALUES(@productId, @barcode, @name, @price, @storeQty, @warehouseQty, @points)";

        //open connection
        checkOpenConnection();
        using (var transaction = connection.BeginTransaction())
        {


            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(query, connection, transaction);
            // prepare statement to avoid SQL injection
            cmd.Prepare();

            // set common values for all the items
            cmd.Parameters.AddWithValue("@productId", product.ID);
            cmd.Parameters.AddWithValue("@barcode", product.Barcode);
            cmd.Parameters.AddWithValue("@name", product.Product_name);
            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.Parameters.AddWithValue("@storeQty", product.StoreQty);
            cmd.Parameters.AddWithValue("@warehouseQty", product.WarehouseQty);
            cmd.Parameters.AddWithValue("@points", product.Points);

            //Execute command
            cmd.ExecuteNonQuery();

            transaction.Commit();

        }
    }
}
