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
using System.Text;
using System.Net.Sockets;
using System.Net;
using GadgeteerHelper;
using System.IO;

namespace HelloLED
{
    public partial class Program
    {

        private Byte[] img;
        private bool autenticato = false;
        private bool showKeypad = false;
        private bool state = false;
        private GT.Timer timer;
        private NumericKeypadHelper numericKeypad;
        private int tentativi = 0;

        private GestioneConnessione gestioneC = new GestioneConnessione();

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            try
            {
                // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
                Debug.Print("Program Started");

                //timer per led lampeggianti
                timer = new GT.Timer(400);
                timer.Tick += timer_led_lampeggianti;
                timer.Start();

                //configuro la videocamera
                camera.BitmapStreamed += stampa_video;
                camera.PictureCaptured += elabora_foto;

                //configuro bottone per scattare foto
                button.ButtonPressed += scatta_foto;
                //configuro bottone per tastiera
                button2.ButtonPressed += button_Button2Pressed;

                //configuro la rete
                ethernetJ11D.NetworkUp += network_up;
                ethernetJ11D.NetworkDown += network_down;
                ethernetJ11D.UseStaticIP("192.168.1.2", "255.255.255.0", "192.168.1.254");
                ethernetJ11D.UseThisNetworkInterface();
                camera.StartStreaming();

                //imposto la suoneria alla pressione dei tasti
                //Tunes.MusicNote note = new Tunes.MusicNote(Tunes.Tone.B2, 200);
                //tunes.AddNote(note);
                tunes.Play(120, 120);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }

        }

        void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            try
            {
                //multicolorLed.BlinkOnce(GT.Color.Blue);
                String text = args.Text;
                tunes.Play(120, 200);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        private void network_down(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            gestioneC.annullaConnessione();

            timer.Stop();
            ledStrip.TurnAllLedsOff();
            ledStrip.TurnLedOn(5);
            ledStrip.TurnLedOn(6);
            //stampo a video un messaggio di errrore (Connessione assente!!!)
        }

        // stampo messaggio di debug quando la connessione si attiva
        // inizializzo il socket e mi collego al server
        private void network_up(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            try
            {
                Debug.Print("Network is up!");
                Debug.Print("My IP is: " + ethernetJ11D.NetworkSettings.IPAddress);

                gestioneC.connetti();

                ledStrip.TurnAllLedsOff();

                if(!timer.IsRunning)
                    timer.Start();
               
            }
            catch (Exception e)
            {
                Debug.Print("Eccezione: " + e);
            }
        }

        //gestione della risposta del server
        //private void req_ResponseReceived(HttpRequest sender, HttpResponse response)
        private void elaboraRisposta(Int32 code)
        {
            Debug.Print("Risposta ricevuta: " + code);
            ledStrip.TurnAllLedsOff();

            if (code == 300)
            {
                displayTE35.SimpleGraphics.Clear();
                displayTE35.SimpleGraphics.DisplayTextInRectangle("Face Detected.\nYou Can Enter", 100, 120, 200, 200, GT.Color.Green, Resources.GetFont(Resources.FontResources.NinaB));
                Debug.Print("Codice 300, volto riconosciuto");
                ledStrip.SetLed(0, true);
                ledStrip.SetLed(1, true);
                tentativi = 0;
                button2.ButtonPressed += button_Button2Pressed;
                autenticato = true;
            }
            else if (code == 400)
            {
                displayTE35.SimpleGraphics.Clear();
                displayTE35.SimpleGraphics.DisplayTextInRectangle("Face Not Detected, Enter Pin.", 60, 120, 200, 200, GT.Color.Red, Resources.GetFont(Resources.FontResources.NinaB));
                Debug.Print("Codice 400, errore, volto non riconosciuto");
                ledStrip.SetLed(5, true);
                ledStrip.SetLed(6, true);
                Thread.Sleep(4000);
                if (tentativi >= 1)
                {
                    button2.ButtonPressed -= button_Button2Pressed;
                    numericKeypad = new NumericKeypadHelper(displayTE35, Resources.GetFont(Resources.FontResources.small));
                    numericKeypad.TextChanged += OnTextChanged;
                    numericKeypad.EnterPressed += OnEnterPressed;
                    showKeypad = true;
                    tentativi = 0;
                    button.ButtonPressed += scatta_foto;
                }
                else
                {
                    tentativi++;
                    resetta_stato();
                }
            }
            else
                //qualsiasi tipo di errore
            {
                displayTE35.SimpleGraphics.Clear();
                displayTE35.SimpleGraphics.DisplayTextInRectangle("Fatal Exception.", 60, 120, 200, 200, GT.Color.Red, Resources.GetFont(Resources.FontResources.NinaB));
                Debug.Print("Errore, codice non riconosciuto");
                ledStrip.SetLed(5, true);
                ledStrip.SetLed(6, true);
                //attivo subito tastiera
                button2.ButtonPressed -= button_Button2Pressed;
                numericKeypad = new NumericKeypadHelper(displayTE35, Resources.GetFont(Resources.FontResources.small));
                numericKeypad.TextChanged += OnTextChanged;
                numericKeypad.EnterPressed += OnEnterPressed;
                showKeypad = true;
                tentativi = 0;
                button.ButtonPressed += scatta_foto;
                
            }

        }

        //elabora ed invia la foto al server
        private void elabora_foto(Camera sender, GT.Picture e)
        {
            //stop blinking leds
            timer.Stop();
            timer.Tick -= timer_led_lampeggianti;
            //stop function to display video on screen
            camera.BitmapStreamed -= stampa_video;
            //stop buttons
            button.ButtonPressed -= scatta_foto;
            button2.ButtonPressed -= button_Button2Pressed;

            if (sender == null && e == null)
            {
                elaboraRisposta(-1);
                return;
            }
            //elabora foto
            img = e.PictureData;

            //stampa su schermo un'immagine fissa (caricamento in corso.../elaborazione...)
            displayTE35.SimpleGraphics.Clear();
            displayTE35.SimpleGraphics.DisplayTextInRectangle("Sending Image", 100, 120, 200, 200, GT.Color.Orange, Resources.GetFont(Resources.FontResources.NinaB));
            ledStrip.SetLed(2, true);
            ledStrip.SetLed(3, true);
            ledStrip.SetLed(4, true);

            DateTime dateI = DateTime.Now;

            Debug.Print("Inizio invio foto: " + dateI.ToString());
            bool flag = gestioneC.ScritturaStream(img);
            if (!flag)
            {
                //errore
                gestioneC.annullaConnessione();
                gestioneC.connetti();
                elaboraRisposta(-1);
                return;
            }
            else
            {

                DateTime dateF = DateTime.Now;
                Debug.Print("Foto inviata: " + dateF.ToString());

                Int32 risposta = gestioneC.LetturaStream();
                if (risposta == -1)
                {
                    //errore
                    gestioneC.annullaConnessione();
                    gestioneC.connetti();
                    elaboraRisposta(-1);
                    return;
                }

                elaboraRisposta(risposta);
            }
        }

        //stampo a video lo streaming
        private void stampa_video(Camera sender, Bitmap e)
        {
            displayTE35.SimpleGraphics.DisplayImage(e, 0, 0);
        }

        //led lampeggianti, per ora lo sono sempre all'avvio
        private void timer_led_lampeggianti(GT.Timer timer)
        {
            state = !state;
            ledStrip.SetLed(2, state);
            ledStrip.SetLed(3, state);
            ledStrip.SetLed(4, state);

        }

        //timer quando il riconoscimento volto ha successo
        private void resetta_stato()
        {
            ledStrip.TurnAllLedsOff();


            timer.Start();
            //restart all functions
            timer.Tick += timer_led_lampeggianti;
            camera.BitmapStreamed += stampa_video;
            button.ButtonPressed += scatta_foto;
            button2.ButtonPressed += button_Button2Pressed;
            try
            {
                if (camera.CameraReady)
                    camera.StartStreaming();
            }
            catch (System.InvalidOperationException e)
            {
                Debug.Print(e.Message);
            }
        }

        //premo il bottone per scattare la foto / per far ripartire lo streaming
        private void scatta_foto(Button sender, Button.ButtonState state)
        {
            if (!showKeypad)
            {
                if (camera.CameraReady)
                {
                    camera.StartStreaming();
                    Debug.Print("Riparte lo streaming");
                    return;
                }
                if (gestioneC.isConnesso())
                {
                    Debug.Print("Stoppo stream e scatto foto");
                    camera.StopStreaming();
                    camera.TakePicture();
                }
                else
                {
                    //errore connessione al server
                    elabora_foto(null, null);
                }
            }
            else
            {
                
                if (gestioneC.isConnesso())
                {
                    Debug.Print("Resetto stato");
                    showKeypad = false;
                    resetta_stato();
                }
                else
                {
                    Debug.Print("Server non connesso");
                }
            }
        }

        
        void button_Button2Pressed(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
        {
            if(autenticato){
                displayTE35.SimpleGraphics.Clear();
                displayTE35.SimpleGraphics.DisplayTextInRectangle("Access Granted.", 100, 120, 200, 200, GT.Color.Cyan, Resources.GetFont(Resources.FontResources.NinaB));
                Thread.Sleep(5000);
                autenticato = false;
                resetta_stato();
            }
            else{
                camera.BitmapStreamed -= stampa_video;
                button.ButtonPressed -= scatta_foto;
                displayTE35.SimpleGraphics.Clear();
                displayTE35.SimpleGraphics.DisplayTextInRectangle("Access Denied\nAuthentication Requested.", 100, 120, 200, 200, GT.Color.Red, Resources.GetFont(Resources.FontResources.NinaB));
                Thread.Sleep(5000);
                camera.BitmapStreamed += stampa_video;
                button.ButtonPressed += scatta_foto;            
            }
        }

        private void OnEnterPressed(object sender, EnterPressedEventArgs args)
        {
            Debug.Print("Enter pressed with value = " + args.Text);
            tunes.Play(80, 200);
            if (args.Text.Equals("0000"))
            {
                displayTE35.SimpleGraphics.Clear();
                displayTE35.SimpleGraphics.DisplayTextInRectangle("Pin Accepted.\nYou Can Enter.", 100, 120, 200, 200, GT.Color.Green, Resources.GetFont(Resources.FontResources.NinaB));
                numericKeypad.TextChanged -= OnTextChanged;
                numericKeypad.EnterPressed -= OnEnterPressed;
                button.ButtonPressed -= scatta_foto;
                showKeypad = false;
                autenticato = true;
                button2.ButtonPressed += button_Button2Pressed;
            }
            else
            {
                displayTE35.SimpleGraphics.Clear();
                displayTE35.SimpleGraphics.DisplayTextInRectangle("Invalid Pin.\nTry Again.", 100, 120, 200, 200, GT.Color.Red, Resources.GetFont(Resources.FontResources.NinaB));
                numericKeypad = new NumericKeypadHelper(displayTE35, Resources.GetFont(Resources.FontResources.small));
                numericKeypad.TextChanged += OnTextChanged;
                numericKeypad.EnterPressed += OnEnterPressed;
                Thread.Sleep(4000);
            }
        }

    }
}
