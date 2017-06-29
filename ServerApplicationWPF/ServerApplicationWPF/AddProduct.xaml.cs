using ServerApplicationWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ServerApplicationWPF
{
    /// <summary>
    /// Logica di interazione per AddProduct.xaml
    /// </summary>
    public partial class AddProduct : Window
    {
        private Product product;
        private DataManager dataManager;
        public AddProduct(Product product, DataManager dataManager)
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
            this.product = product;
            this.dataManager = dataManager;
            BarcodeValue.Content = product.Barcode;
            NameValue.Text = product.Product_name;
            PriceValue.Text = product.Price;
            PointsValue.Text = product.Points.ToString();
            StoreQtyValue.Text = product.StoreQty.ToString();
            WarehouseQtyValue.Text = product.WarehouseQty.ToString();
        }


        private void IntNumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("\\d");
            e.Handled = !regex.IsMatch(e.Text);
        }
        private void PriceNumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[\\d,]");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            product.Product_name = NameValue.Text;
            product.Price = PriceValue.Text;
            product.Points = PointsValue.Text;
            double price = double.Parse(product.Price);
            product.StoreQty = int.Parse(StoreQtyValue.Text);
            product.WarehouseQty = int.Parse(WarehouseQtyValue.Text);

            if (price <= 0)
            {
                MessageBox.Show("Price must be greater than 0");
            }
            else if (product.Product_name == "")
            {
                MessageBox.Show("Provide a name for the product");
            }
            else
            {
                try
                {
                    dataManager.insertProduct(product);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while inserting the new product in the database!\n" + ex.Message);
                }
            }
        }
    }
}
