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
            ethernetJ11D.UseStaticIP("192.168.1.2", "255.255.255.0", "0.0.0.0");
            ethernetJ11D.UseThisNetworkInterface();
            //ethernetJ11D.UseStaticIP("") I have to configure!
            ethernetJ11D.NetworkUp += ethernetJ11D_NetworkUp;
            ethernetJ11D.NetworkDown += ethernetJ11D_NetworkDown;
            //new Thread(RunWebServer).Start();
            //I call the function ConnectionManagement;
            // instantiate the connection management object
            connection = new ConnectionManagement();
            
            button.ButtonPressed += new Button.ButtonEventHandler(button_ButtonPressed);
            camera.PictureCaptured += new Camera.PictureCapturedEventHandler(camera_PictureCaptured);


            Debug.Print("Program Started");

            //welcome tune
            
            Tunes.MusicNote[] notes = new Tunes.MusicNote[4];
            notes[0] = new Tunes.MusicNote(Tunes.Tone.C4, 150);
            notes[1] = new Tunes.MusicNote(Tunes.Tone.E4, 150);
            notes[2] = new Tunes.MusicNote(Tunes.Tone.G4, 150);
            notes[3] = new Tunes.MusicNote(Tunes.Tone.C5, 300);

            tunes.Play(notes);
            

        }

        private void camera_PictureCaptured(Camera sender, GT.Picture picture)
        {
            displayTE35.SimpleGraphics.DisplayImage(picture, 5, 5);
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
            } else
            {
                Debug.Print("Not connected, not sending");
            }
        }

        private void button_ButtonPressed(Button sender, Button.ButtonState state)
        {
            tunes.Play(1100, 300);
            camera.TakePicture();
        }

        void ethernetJ11D_NetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is down!");
            connection.annullaConnessione();
        }

        void ethernetJ11D_NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is up!");
            Debug.Print("My IP is: " + ethernetJ11D.NetworkSettings.IPAddress);
            connection.Connect();
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