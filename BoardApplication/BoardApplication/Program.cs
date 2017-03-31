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
using System.MarshalByRefObject;
using System.Drawing;
using System.Drawing.Image;
using System.Drawing.Bitmap;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;


namespace BoardApplication
{
    public partial class Program
    {
        private ConnectionManagement connection;
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

            ethernetJ11D.UseThisNetworkInterface();
            //ethernetJ11D.UseStaticIP("") I have to configure!
            ethernetJ11D.NetworkUp += ethernetJ11D_NetworkUp;
            ethernetJ11D.NetworkDown += ethernetJ11D_NetworkDown;
            //new Thread(RunWebServer).Start();
            //I call the function ConnectionManagement;
            
            button.ButtonPressed += new Button.ButtonEventHandler(button_ButtonPressed);
            camera.PictureCaptured += new Camera.PictureCapturedEventHandler(camera_PictureCaptured);
            Debug.Print("Program Started");


        }

        private void camera_PictureCaptured(Camera sender, GT.Picture picture)
        {
            displayTE35.SimpleGraphics.DisplayImage(picture, 5, 5);
            connection.Connect();
            if (connection.isConnected() == true)
            {
                //Bitmap picture_Captured = new Bitmap(picture, Bitmap.BitmapImageType.Bmp);
                // allocate buffer
                byte[] img = picture.PictureData;
                if (connection.WriteStream(img) == true)
                {
                    Debug.Print("The image has been sent correctly to the server!");
                }
                else Debug.Print("The image has been sent wrongly to the server");
            }
        }

        private void button_ButtonPressed(Button sender, Button.ButtonState state)
        {
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
        }

        void RunWebServer()
        {
            // Wait for the network...
            while (ethernetJ11D.IsNetworkUp == false)
            {
                Debug.Print("Waiting...");
                Thread.Sleep(1000);
            }
        }
    }
}