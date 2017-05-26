using Gadgeteer.Modules.GHIElectronics;
using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;
using Microsoft.SPOT.Input;
using System.IO;
using System.Net;
using System.Text;
using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Json.NETMF;

namespace BoardApplication
{
    public partial class Program
    {
        //private ConnectionManagement connection;
        //Variable used to display the GUI.
        private Client client;
        private Text txtMessage;
        private Bitmap normalButton;
        private Bitmap pressedButton;
        private Image imgButton;
        private Boolean flagButtonPressHere = false;
        private Window window;
        private Hashtable l = new Hashtable();
        private int flagThread = 0;
        private HttpWebRequest clientReq;
        private int WindowGlod = 0;
        private Boolean barcodeError = false;
        private GT.Picture picture;
        private UserInfo user;
        private Boolean globalAuth = false;
        private Boolean firstPicture = true;
        private String stringError = "Error: repeat the scanning of the picture";
        
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {    
            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
             String[] array = {"192.168.1.1"};
             ethernetJ11D.NetworkInterface.Open();
             ethernetJ11D.NetworkInterface.EnableStaticIP("192.168.1.2", "255.255.255.0", "192.168.1.1");
             ethernetJ11D.NetworkInterface.EnableStaticDns(array);
             
             ethernetJ11D.NetworkUp += ethernetJ11D_NetworkUp;
             ethernetJ11D.NetworkDown += ethernetJ11D_NetworkDown;

             while (ethernetJ11D.NetworkInterface.IPAddress == "0.0.0.0")
             {
                 Debug.Print("Waiting for DHCP");
                 Thread.Sleep(250);
             }

            camera.PictureCaptured += new Camera.PictureCapturedEventHandler(camera_PictureCaptured);
            camera.TakePicture();
           // camera.TakePicture();
            button.ButtonPressed += new Button.ButtonEventHandler(button_ButtonPressed);
                          
            Debug.Print("Program Started");

            //welcome tune
        /*    
            Tunes.MusicNote[] notes = new Tunes.MusicNote[4];
            notes[0] = new Tunes.MusicNote(Tunes.Tone.C4, 150);
            notes[1] = new Tunes.MusicNote(Tunes.Tone.E4, 150);
            notes[2] = new Tunes.MusicNote(Tunes.Tone.G4, 150);
            notes[3] = new Tunes.MusicNote(Tunes.Tone.C5, 300);

            tunes.Play(notes);*/

            window = displayTE35.WPFWindow;

            
            createWindowOne();
         
        }

      
        //Touch event linked to the first window.
        void imgButton_TouchUp(object sender, TouchEventArgs e)
        {
            imgButton.Bitmap = normalButton;
            if (flagButtonPressHere == true)
            {
                flagButtonPressHere = false;
                createWindowTwo();

            }
        }

        void imgButton_TouchDown(object sender, TouchEventArgs e)
        {
            imgButton.Bitmap = pressedButton;
            flagButtonPressHere = true;
        }

        //FIRST WINDOW
        void createWindowOne()
        {
            user = null;
            globalAuth = false;
            barcodeError = false;
            flagButtonPressHere = false;
            byte[] normalButtonByte;
            byte[] pressedButtonByte;
            Canvas canvas = new Canvas();
            window.Child = canvas;
            window.Background = new SolidColorBrush(GT.Color.White);
            WindowGlod = 1;

            l.Clear();
            Font baseFont = Resources.GetFont(Resources.FontResources.NinaB);
            
            
            txtMessage = new Text(baseFont, "Welcome to the automatic cash.");

            canvas.Children.Add(txtMessage);
            Canvas.SetTop(txtMessage, 30);
            Canvas.SetLeft(txtMessage, 45);
            txtMessage = new Text(baseFont, "Select your products pressing the");
            
            canvas.Children.Add(txtMessage);
            Canvas.SetTop(txtMessage, 50);
            Canvas.SetLeft(txtMessage, 40);
            txtMessage = new Text(baseFont, "button 'Press Here'!");
            canvas.Children.Add(txtMessage);
            Canvas.SetTop(txtMessage, 65);
            Canvas.SetLeft(txtMessage, 90);

            normalButtonByte = Resources.GetBytes(Resources.BinaryResources.NormalButton);
            pressedButtonByte = Resources.GetBytes(Resources.BinaryResources.PressedButton);
            normalButton = new Bitmap(normalButtonByte, Bitmap.BitmapImageType.Jpeg);
            pressedButton = new Bitmap(pressedButtonByte, Bitmap.BitmapImageType.Jpeg);

            normalButton.SetPixel(154, 55, GT.Color.Blue);
            imgButton = new Image(normalButton);
            canvas.Children.Add(imgButton);
            Canvas.SetTop(imgButton, 110);
            Canvas.SetLeft(imgButton, 80);
            imgButton.TouchDown += new TouchEventHandler(imgButton_TouchDown);
            imgButton.TouchUp += new TouchEventHandler(imgButton_TouchUp);
        }

        //SECOND WINDOW
        void createWindowTwo()
        {
            WindowGlod = 2;
            Canvas canvas = new Canvas();
            window.Child = canvas;
            Font baseFont = Resources.GetFont(Resources.FontResources.NinaB);
                
            if (globalAuth == false)
            {
                txtMessage = new Text(baseFont, "SCAN YOUR QR CODE ");
                Canvas.SetTop(txtMessage, 70);
                Canvas.SetLeft(txtMessage, 90);
                
                canvas.Children.Add(txtMessage);
                txtMessage = new Text(baseFont, "IN FRONT OF ");
                Canvas.SetTop(txtMessage, 100);
                Canvas.SetLeft(txtMessage, 108);
                canvas.Children.Add(txtMessage);
                txtMessage = new Text(baseFont, "THE CAMERA");
                Canvas.SetTop(txtMessage, 130);
                Canvas.SetLeft(txtMessage, 110);
                canvas.Children.Add(txtMessage);
            }
            else
            {
                txtMessage = new Text(baseFont, "The authentication is failed.");
                Canvas.SetTop(txtMessage, 30);
                Canvas.SetLeft(txtMessage, 50);
                canvas.Children.Add(txtMessage);
                txtMessage = new Text(baseFont, "Please scan again your QR code ");
                Canvas.SetTop(txtMessage, 75);
                Canvas.SetLeft(txtMessage, 50);
                canvas.Children.Add(txtMessage);
                txtMessage = new Text(baseFont, "in front of the camera.");
                Canvas.SetTop(txtMessage, 95);
                Canvas.SetLeft(txtMessage, 75);
                canvas.Children.Add(txtMessage);
            }
                 
        }

        //Touch linked to the second window
        void imgButton_TouchUp2(object sender, TouchEventArgs e)
        {
            imgButton.Bitmap = normalButton;
            if (flagButtonPressHere == true)
            {
                flagButtonPressHere = false;
                createWindowThree();

            }
        }

        void imgButton_TouchDown2(object sender, TouchEventArgs e)
        {
            imgButton.Bitmap = pressedButton;
            flagButtonPressHere = true;
        }

        //WINDOW THREE
        void createWindowThree()
        {
            byte[] normalButtonByte;
            byte[] pressedButtonByte;
            byte[] deleteNormalButtonByte;
            byte[] deletePressedButtonByte;
           

            Canvas canvas = new Canvas();
            window.Child = canvas;
            Font baseFont = Resources.GetFont(Resources.FontResources.NinaB);
            txtMessage = new Text(baseFont, "List of goods:");
            Canvas.SetTop(txtMessage, 10);
            Canvas.SetLeft(txtMessage, 30);
            canvas.Children.Add(txtMessage);

            Image[] deleteButton = new Image[l.Count];
            
            WindowGlod = 3;
          
            int top = 30;
            int left = 30;
            int i=0;
            foreach (DictionaryEntry d in l)
            {

                deleteNormalButtonByte = Resources.GetBytes(Resources.BinaryResources.NormalDelete);
                deletePressedButtonByte = Resources.GetBytes(Resources.BinaryResources.PressedDelete);

                Bitmap bitmapNormalButton = new Bitmap(deleteNormalButtonByte, Bitmap.BitmapImageType.Jpeg);
                Bitmap bitmapPressedButton = new Bitmap(deletePressedButtonByte, Bitmap.BitmapImageType.Jpeg);
                bitmapNormalButton.SetPixel(10, 10, GT.Color.Blue);
                deleteButton[i] = new Image(bitmapNormalButton);
                
                deleteButton[i].TouchDown += new TouchEventHandler(imgButton_TouchDownDelete);
                deleteButton[i].TouchUp += new TouchEventHandler(imgButton_TouchUpDelete);
                
                ProductInfo p = d.Value as ProductInfo;
                String s = p.productName + " " + p.price + "$" + "    " + p.Qty+"-";


                Canvas.SetTop(deleteButton[i], top);
                Canvas.SetLeft(deleteButton[i], 150);
                canvas.Children.Add(deleteButton[i]);
                i++;
                txtMessage = new Text(baseFont,s);

                Canvas.SetTop(txtMessage, top);
                Canvas.SetLeft(txtMessage, left);
                canvas.Children.Add(txtMessage);
                top += 15;
            }

            if (barcodeError == true)
            {
                txtMessage = new Text(baseFont, stringError);
                Canvas.SetTop(txtMessage, top);
                Canvas.SetLeft(txtMessage, left);
                canvas.Children.Add(txtMessage);
                top += 15;
            }
            

            normalButtonByte = Resources.GetBytes(Resources.BinaryResources.payButton);
            pressedButtonByte = Resources.GetBytes(Resources.BinaryResources.payButtonPressed);
            normalButton = new Bitmap(normalButtonByte, Bitmap.BitmapImageType.Jpeg);
            pressedButton = new Bitmap(pressedButtonByte, Bitmap.BitmapImageType.Jpeg);
            normalButton.SetPixel(154, 55, GT.Color.Blue);
            imgButton = new Image(normalButton);
            Canvas.SetTop(imgButton, 170);
            Canvas.SetLeft(imgButton, 80);
            canvas.Children.Add(imgButton);

            imgButton.TouchDown += new TouchEventHandler(imgButton_TouchDown3);
            imgButton.TouchUp += new TouchEventHandler(imgButton_TouchUp3);

            //I send the list to the web service:
         /*   for (i = 0; i < l.Count; i++)
            {
                byte[] productBytes = Encoding.UTF8.GetBytes(l[i].ToString());
                client.sendBytes(productBytes);
            }            */
        }

        private void imgButton_TouchDownDelete(object sender, TouchEventArgs e)
        {
                       
            



        }

        private void imgButton_TouchUpDelete(object sender, TouchEventArgs e)
        {
            
        }

        //Touch linked to window three
        void imgButton_TouchUp3(object sender, TouchEventArgs e)
        {
            imgButton.Bitmap = normalButton;
            if (flagButtonPressHere == true)
            {
                flagButtonPressHere = false;
                int i;
                ArrayList list = new ArrayList();
                foreach (DictionaryEntry d in l)
                {
                    ProductInfo p = d.Value as ProductInfo;
                    String productId = p.IDProduct;

                    Double qty = p.Qty;
                    Hashtable hashtable = new Hashtable();

                    hashtable.Add("ID", productId);
                    hashtable.Add("Qty", qty);
                    list.Add(hashtable);
                }

                Hashtable receiptTable = new Hashtable();
                receiptTable.Add("UserID", user.UserID);
                receiptTable.Add("List", list);
                string json = JsonSerializer.SerializeObject(receiptTable);
                int token = client.AskToken();
                byte[] productBytes = Encoding.UTF8.GetBytes(json);
                client.SendData(productBytes, token);

                if (Utils.BytesToString(client.ReceiveData(token)).Equals("OK"))
                {
                    l.Clear();
                    createWindowFour();
                }
                else flagButtonPressHere = false;
                //createWindowOne();
            }
        }

        void imgButton_TouchDown3(object sender, TouchEventArgs e)
        {
            imgButton.Bitmap = pressedButton;
            flagButtonPressHere = true;
        }

        void createWindowFour()
        {
            GT.Timer timer = new GT.Timer(3000); // Create a timer
            Canvas canvas = new Canvas();
            window.Child = canvas;
            Font baseFont = Resources.GetFont(Resources.FontResources.NinaB);
            txtMessage = new Text(baseFont, "THANKS FOR YOUR PURCHASE!");
            Canvas.SetTop(txtMessage, 100);
            Canvas.SetLeft(txtMessage, 70);
            canvas.Children.Add(txtMessage);

            
            timer.Tick += timer_Tick; // Run the method timer_tick when the timer ticks
            timer.Start(); // Start the timer
        }
       

        void timer_Tick(GT.Timer timer){
            timer.Stop();
            createWindowOne();
        }

        private void camera_PictureCaptured(Camera sender,GT.Picture foto)
        {
            
            picture = foto;
            if (firstPicture == false)
            {
                Thread t = new Thread(configurePicture);

                t.Start();
                t.Join();
                if (WindowGlod == 3)
                    createWindowThree();
                else
                {
                    if (user == null)
                        createWindowTwo();
                    else createWindowPurchase();
                }
            }
            else firstPicture = false;
        }

        private void createWindowPurchase()
        {
            
            byte[] normalButtonByte;
            byte[] pressedButtonByte;
            Canvas canvas = new Canvas();
            window.Child = canvas;
            window.Background = new SolidColorBrush(GT.Color.White);
            
            Font baseFont = Resources.GetFont(Resources.FontResources.NinaB);

            txtMessage = new Text(baseFont, "Welcome " + user.name);
            canvas.Children.Add(txtMessage);
            Canvas.SetTop(txtMessage, 80);
            Canvas.SetLeft(txtMessage, 40);

            normalButtonByte = Resources.GetBytes(Resources.BinaryResources.startBuying);
            pressedButtonByte = Resources.GetBytes(Resources.BinaryResources.PressedStartBuying);
            normalButton = new Bitmap(normalButtonByte, Bitmap.BitmapImageType.Jpeg);
            pressedButton = new Bitmap(pressedButtonByte, Bitmap.BitmapImageType.Jpeg);
            normalButton.SetPixel(154, 55, GT.Color.Blue);
            imgButton = new Image(normalButton);
            Canvas.SetTop(imgButton, 110);
            Canvas.SetLeft(imgButton, 80);
            canvas.Children.Add(imgButton);
            WindowGlod = 2;
            imgButton.TouchDown += new TouchEventHandler(imgButton_TouchDown2);
            imgButton.TouchUp += new TouchEventHandler(imgButton_TouchUp2);
        }

        private void configurePicture()
        {
            byte[] result = new byte[65536];
            int read = 0;
            result = picture.PictureData;

            Boolean receivedToken = false;
            int token = 0;
            while (receivedToken == false)
            {
                try
                {
                    token = client.AskToken();
                    client.SendData(result, token);
                    receivedToken = true;
                }
                catch (Exception e)
                {

                }
            }

            byte[] receivedMessage = client.ReceiveData(token);
            //Debug.Print(Utils.BytesToString(receivedMessage));
            if (barcodeError == true)
            {
             //   l.Remove("Error");
                barcodeError = false;
            }
            if (Utils.BytesToString(receivedMessage).Equals("Error"))
            {
                barcodeError = true;
                
                if (WindowGlod == 2)
                    globalAuth = true;
            }
            else
            {
                Hashtable hashTable = JsonSerializer.DeserializeString(Utils.BytesToString(receivedMessage)) as Hashtable;

                if (WindowGlod == 2)
                {
                    user = new UserInfo();
                    user.name = hashTable["Name"] as String;
                    user.surname = hashTable["Surname"] as String;
                    user.UserID = hashTable["ID"] as String;
                }
                else
                {
                    ProductInfo prod = new ProductInfo();
                    prod.IDProduct = hashTable["ID"] as String;
                    if (!l.Contains(prod.IDProduct))
                    {
                        prod.productName = hashTable["Product_name"] as String;
                        String priceString = hashTable["Price"] as String;
                        prod.price = Double.Parse(priceString);
                        String pointString = hashTable["Points"] as String;
                        prod.points = Double.Parse(pointString);
                        prod.Qty++;
                        l.Add(prod.IDProduct, prod);
                    }
                    else
                    {
                        (l[prod.IDProduct] as ProductInfo).Qty++;
                    }
                }
            }
        }

        private void button_ButtonPressed(Button sender, Button.ButtonState state)
        {
            // tunes.Play(1100, 300);
            if (WindowGlod == 3 || WindowGlod == 2) 
                camera.TakePicture();      
        }

        void ethernetJ11D_NetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is down!");
        }

        void ethernetJ11D_NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is up!");
            Debug.Print("My IP is: " + ethernetJ11D.NetworkSettings.IPAddress);
            IPEndPoint IPaddress = new IPEndPoint(IPAddress.Parse("192.168.1.1"), 8000);
            client = new Client(IPaddress);
        }

    }
}