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
        private ArrayList l = new ArrayList();
        private int flagThread = 0;
        private HttpWebRequest clientReq;
        
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/



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
             button.ButtonPressed += new Button.ButtonEventHandler(button_ButtonPressed);
             Thread.Sleep(1000);
             IPEndPoint IPaddress = new IPEndPoint(IPAddress.Parse("192.168.1.1"), 8000);
             client = new Client(IPaddress);
           

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
            byte[] normalButtonByte;
            byte[] pressedButtonByte;
            Canvas canvas = new Canvas();
            window.Child = canvas;
            window.Background = new SolidColorBrush(GT.Color.White);


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

            normalButton.SetPixel(20, 20, GT.Color.Blue);
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
            Canvas canvas = new Canvas();
            window.Child = canvas;
            Font baseFont = Resources.GetFont(Resources.FontResources.NinaB);
            txtMessage = new Text(baseFont, "Scan your product in front of");
            Canvas.SetTop(txtMessage, 30);
            Canvas.SetLeft(txtMessage, 50);
            canvas.Children.Add(txtMessage);
            txtMessage = new Text(baseFont, "the camera pressing");
            Canvas.SetTop(txtMessage, 45);
            Canvas.SetLeft(txtMessage, 90);
            canvas.Children.Add(txtMessage);
            txtMessage = new Text(baseFont, "the button 'Start buying'");
            Canvas.SetTop(txtMessage, 60);
            Canvas.SetLeft(txtMessage, 75);
            canvas.Children.Add(txtMessage);

            byte[] normalButtonByte;
            byte[] pressedButtonByte;

            normalButtonByte = Resources.GetBytes(Resources.BinaryResources.startBuying);
            pressedButtonByte = Resources.GetBytes(Resources.BinaryResources.PressedStartBuying);
            normalButton = new Bitmap(normalButtonByte, Bitmap.BitmapImageType.Jpeg);
            pressedButton = new Bitmap(pressedButtonByte, Bitmap.BitmapImageType.Jpeg);
            normalButton.SetPixel(20, 20, GT.Color.Blue);
            imgButton = new Image(normalButton);
            Canvas.SetTop(imgButton, 110);
            Canvas.SetLeft(imgButton, 80);
            canvas.Children.Add(imgButton);

            imgButton.TouchDown += new TouchEventHandler(imgButton_TouchDown2);
            imgButton.TouchUp += new TouchEventHandler(imgButton_TouchUp2);

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
            Canvas canvas = new Canvas();
            window.Child = canvas;
            Font baseFont = Resources.GetFont(Resources.FontResources.NinaB);
            txtMessage = new Text(baseFont, "List of goods:");
            Canvas.SetTop(txtMessage, 10);
            Canvas.SetLeft(txtMessage, 30);
            canvas.Children.Add(txtMessage);


           
          //  Here I develop the communication in order to get the objects
       /*
            int i = 2, tot = 0;
            l.Add("Bread          " + i.ToString() + "$");
            tot += i;
            i++;
            l.Add("Potatos          " + i.ToString() + "$");
            tot += i;
            i++;
            l.Add("Chocolate          " + i.ToString() + "$");
            tot += i;
            i++;
            l.Add("Mais          " + i.ToString() + "$");

            tot += i;
            i++;
            l.Add("Apple         " + i.ToString() + "$");
            tot += i;
            i++;
            l.Add("Salad         " + i.ToString() + "$");
            tot += i;
            i++;
           */

            int top = 30;
            int left = 30;
            int i;
            for (i = 0; i < l.Count; i++)
            {
                txtMessage = new Text(baseFont, l[i].ToString());
                Canvas.SetTop(txtMessage, top);
                Canvas.SetLeft(txtMessage, left);
                canvas.Children.Add(txtMessage);
                top += 15;
            }
            byte[] normalButtonByte;
            byte[] pressedButtonByte;


            normalButtonByte = Resources.GetBytes(Resources.BinaryResources.payButton);
            pressedButtonByte = Resources.GetBytes(Resources.BinaryResources.payButtonPressed);
            normalButton = new Bitmap(normalButtonByte, Bitmap.BitmapImageType.Jpeg);
            pressedButton = new Bitmap(pressedButtonByte, Bitmap.BitmapImageType.Jpeg);
            normalButton.SetPixel(20, 20, GT.Color.Blue);
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

        //Touch linked to window three
        void imgButton_TouchUp3(object sender, TouchEventArgs e)
        {
            imgButton.Bitmap = normalButton;
            if (flagButtonPressHere == true)
            {
                flagButtonPressHere = false;
               // createWindowFour(); This window doesn't appear on the screen.

                createWindowOne();
            }
        }

        void imgButton_TouchDown3(object sender, TouchEventArgs e)
        {
            imgButton.Bitmap = pressedButton;
            flagButtonPressHere = true;
        }

        void createWindowFour()
        {

            Canvas canvas = new Canvas();
            window.Child = canvas;
            Font baseFont = Resources.GetFont(Resources.FontResources.NinaB);
            txtMessage = new Text(baseFont, "Thanks for your purchase!");
            Canvas.SetTop(txtMessage, 10);
            Canvas.SetLeft(txtMessage, 70);
            canvas.Children.Add(txtMessage);

        }
       
        private void camera_PictureCaptured(Camera sender, GT.Picture picture)
        {
            byte[] result = new byte[65536];
            int read = 0;
            result = picture.PictureData;

          //  client = new Client();

          //  client.sendBytes(result);
           int token = client.AskToken();
           client.SendData(result,token);

           byte[] receivedMessage = client.ReceiveData(token);
        }
        

        private void button_ButtonPressed(Button sender, Button.ButtonState state)
        {
           // tunes.Play(1100, 300);
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
            ethernetJ11D.NetworkInterface.Open();

        }

    }
}