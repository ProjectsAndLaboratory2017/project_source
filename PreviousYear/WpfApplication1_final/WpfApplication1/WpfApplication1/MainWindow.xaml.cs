using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using MySql.Data.MySqlClient;

namespace WpfApplication1
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Thread threadSocket;
        Byte[] data = null;

        private Socket sClient = null;
        private Socket server = null;

        private delegate void modificaMessaggio(String msg, bool clear);
        //This is my connection string i have assigned the database file address path  
        string databaseConnection = "datasource=localhost;port=3306;username=gadgeteer;password=gadgeteer";  


        public MainWindow()
        {
            InitializeComponent();
            try
            {
                threadSocket = new Thread(new ParameterizedThreadStart(start_listener));
                threadSocket.Start();
            }
            catch (Exception e)
            {
                Debug.Print("Exception: " + e);
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                if (sClient != null)
                {
                    sClient.Close();
                }
                if (server != null)
                {
                    server.Close();
                }
                if (threadSocket != null)
                {
                    threadSocket.Abort();
                }
            }
            catch (Exception)
            {
                Debug.Print("Exception: " + e);
                return;
            }
        }

        private void start_listener(object obj)
        {
            try
            {
                //debug message
                String messaggio;

                //set sockets permissions in windows
                /*SocketPermission permission = new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", SocketPermission.AllPorts);
                permission.Demand();*/

                //create server socket bind and start listen
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 255);
                IPEndPoint endP = new IPEndPoint(IPAddress.Any, 8000);
                server.Bind(endP);
                server.Listen(10);

                //accept a client
                //sClient.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 255);
                sClient = server.Accept();
                
                //print to screen message when socket is accepted
                messaggio = "Socket accettato " + sClient.RemoteEndPoint;
                //metto in coda l'aggiornamento dell'interfaccia grafica
                this.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                new modificaMessaggio(stampaTitolo), messaggio, true);

                while (true)
                {
                    
                    //ad ogni ciclo deve controllare se è stata richiesta la chiusura
                    messaggio = "In attesa della foto ";
                    //metto in coda l'aggiornamento dell'interfaccia grafica
                    this.Dispatcher.BeginInvoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new modificaMessaggio(stampaMsg), messaggio, true);


                    letturaSocket(sClient);

                    /*
                    inserisciFoto(foto);
                    int id = recuperaUltimoId();
                    Debug.Print("id: %d", id);
                    recuperaFoto(id);
                     */

                    messaggio = "Foto letta" + DateTime.Now.ToString("mm:ss.fff");
                    //metto in coda l'aggiornamento dell'interfaccia grafica
                    this.Dispatcher.BeginInvoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new modificaMessaggio(stampaMsg), messaggio, false);

                    bool risp = elaboraFoto(data);
                    int code;
                    if(risp){
                        code = 300;
                        messaggio = "Volto rilevato";
                        //metto in coda l'aggiornamento dell'interfaccia grafica
                        this.Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new modificaMessaggio(stampaMsg), messaggio, false);
                    } else {
                        code = 400;
                        messaggio = "Volto non rilevato";
                        //metto in coda l'aggiornamento dell'interfaccia grafica
                        this.Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new modificaMessaggio(stampaMsg), messaggio, false);
                    }
                    ScritturaStream(sClient, code);
                    Thread.Sleep(4000);
                }
            }
            catch (SocketException se)
            {
                Debug.Print("SocketException: " + se.Message);
            }
            catch (Exception e)
            {
                Debug.Print("Exception: " + e.Message);
                if (sClient != null)
                    sClient.Close();
                if (server != null)
                    server.Close();
            }

        }

        private void stampaInizio()
        {
            String messaggio = "Inizio ricezione: " + DateTime.Now.ToString("mm:ss.fff");
            //metto in coda l'aggiornamento dell'interfaccia grafica
            this.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                new modificaMessaggio(stampaMsg), messaggio, true);
        }

        private bool ScritturaStream(Socket s, Int32 code)
        {
            try
            {
                int count = sizeof(Int32);
                int i = 0, byteInviati;
                Byte[] sData = BitConverter.GetBytes(code);
                while (count > 0)
                {
                    byteInviati = s.Send(sData, i, count, SocketFlags.None);
                    count -= byteInviati;
                    i += byteInviati;
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                return false;
            }
        }

        private void letturaSocket(Socket s)
        {
            Int32 bytesRead = 0;
            int count;
            int i = 0;
            int numByte;

            try
            {
                //leggo prima la dimensione 32 bit poi la stringa
                data = new Byte[sizeof(int)];
                count = sizeof(int);
                numByte = sizeof(int);
                while (count > 0)
                {
                    bytesRead = s.Receive(data, i, count, SocketFlags.None);
                    i += bytesRead;
                    count -= bytesRead;
                }
                numByte = BitConverter.ToInt32(data, 0);

                //leggo la stringa
                data = new Byte[numByte];
                count = data.Length;
                i = 0;
                bytesRead = 0;
                while (count > 0)
                {
                    try
                    {
                        bytesRead = s.Receive(data, i, count, SocketFlags.None);
                        i += bytesRead;
                        count -= bytesRead;
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.WouldBlock ||
                            ex.SocketErrorCode == SocketError.IOPending ||
                            ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                        {
                            // socket buffer is probably empty, wait and try again
                            Thread.Sleep(30);
                        }
                        else
                            throw ex;  // any serious error occurr
                    }
                }

                //return data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private int recuperaUltimoId()
        {
            string sql = "SELECT id FROM gadgeteer.photos ORDER BY id DESC LIMIT 1";
            int id;
            using (var conn = new MySqlConnection(databaseConnection))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    //cmd.Parameters.AddWithValue("@ID", id);
                    //byte[] bytes = (byte[])cmd.ExecuteScalar();
                    id = (int)cmd.ExecuteScalar();
                }
            }
            return id;
        }

        private void recuperaFoto(int id)
        {
            // read image from database like this
            string sql = "SELECT photo FROM gadgeteer.photos WHERE ID = @ID";
            using (var conn = new MySqlConnection(databaseConnection))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    byte[] bytes = (byte[])cmd.ExecuteScalar();
                    using (var byteStream = new MemoryStream(bytes))
                    {
                        //pictureBox1.Image = new Bitmap(byteStream);
                        //Bitmap bmp = new Bitmap(bytes);
                        Image<Bgr, byte> image = new Image<Bgr, byte>(new Bitmap(byteStream));
                        image.Save("resultDb.bmp");
                    }
                }
            }
        }

        private void inserisciFoto(byte[] img, bool status)
        {
            string sql = "INSERT INTO gadgeteer.photos(photo, status) VALUES (@file, @status)";
            using (var conn = new MySqlConnection(databaseConnection))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    // parameterize query to safeguard against sql injection attacks, etc. 
                    cmd.Parameters.AddWithValue("@file", img);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private bool elaboraFoto(byte[] img)
        {
            try
            {

                Emgu.CV.Image<Bgr, byte> image;
                using (var byteStream = new MemoryStream(img))
                {
                    image = new Emgu.CV.Image<Bgr, byte>(new Bitmap(byteStream));
                }
                 
                long detectionTime;
                List<System.Drawing.Rectangle> faces = new List<System.Drawing.Rectangle>();
                List<System.Drawing.Rectangle> eyes = new List<System.Drawing.Rectangle>();

                //The cuda cascade classifier doesn't seem to be able to load "haarcascade_frontalface_default.xml" file in this release
                //disabling CUDA module for now
                bool tryUseCuda = false;
                /*Per vedere se sono state rilevate facce o occhi eventualmente possiamo mettere
                 * un contatore che ci dice quante facce o occhi ci sono
                 */
                bool face_detect = false;
                bool eye_detect = false;

               
                DetectFace.Detect(
                  image.Mat, "haarcascade_frontalface_default.xml", "haarcascade_eye.xml",
                  faces, eyes,
                  tryUseCuda,
                  out detectionTime);

                foreach (System.Drawing.Rectangle face in faces)
                {
                    CvInvoke.Rectangle(image, face, new Bgr(System.Drawing.Color.Red).MCvScalar, 2);
                    face_detect = true;
                }
                foreach (System.Drawing.Rectangle eye in eyes)
                {
                    CvInvoke.Rectangle(image, eye, new Bgr(System.Drawing.Color.Blue).MCvScalar, 2);
                    eye_detect = true;
                }

                image.Save("Result.bmp");

                Bitmap result = image.ToBitmap();
                byte[] xByte = ToByteArray(result, ImageFormat.Bmp);
                

            

                int id;
                //qui salva la foto solo se è stata rilevata una faccia o degli occhi
                if (face_detect || eye_detect)
                {
                    inserisciFoto(xByte, true);
                    id = recuperaUltimoId();
                    recuperaFoto(id);
                    return true;
                }
                else
                {
                    inserisciFoto(xByte, false);
                    id = recuperaUltimoId();
                    recuperaFoto(id);
                    return false;
                }
                
            }

            catch (Exception e)
            {
                Debug.Print("Exception: " + e);
                return false;
            }
        }

        private void stampaMsg(String msg, bool clear)
        {
            try
            {
                if (clear)
                {
                    testo.Content = msg;
                }
                else
                {
                    testo.Content += "\n" + msg;
                }
               
            } catch (Exception e){

                Debug.Print("Exception: " + e.Message);
            }
            
        }

        private void stampaTitolo(String msg, bool clear)
        {
            try
            {
                titolo.Content = msg;

            }
            catch (Exception e)
            {

                Debug.Print("Exception: " + e.Message);
            }

        }

        private void resettaServer(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sClient != null)
                {
                    sClient.Close();
                }
                if (server != null)
                {
                    server.Close();
                }
                if (threadSocket != null)
                {
                    threadSocket.Abort();
                }

                //faccio ripartire il server
                titolo.Content = "In attesa della connessione:";
                threadSocket = new Thread(new ParameterizedThreadStart(start_listener));
                threadSocket.Start();
            }
            catch (Exception)
            {
                Debug.Print("Exception: " + e);
                return;
            }
        }

        public static byte[] ToByteArray(System.Drawing.Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }

        

    }
}
