using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace HelloLED
{
    class GestioneConnessione
    {
        private Socket clientS = null;
        private bool inConnessione = false;
        private bool connesso = false;
        private IPEndPoint endPoint;

        public GestioneConnessione()
        {
            endPoint = new IPEndPoint(IPAddress.Parse("192.168.1.1"), 8000);
        }

        public void connetti()
        {
            if (!inConnessione && !connesso)
            {
                try
                {
                    Debug.Print("Nuovo thread per connettersi al server");
                    inConnessione = true;

                    ThreadStart threadStart = new ThreadStart(connettiServer);
                    Thread thread = new Thread(threadStart);
                    thread.Start();
                }
                catch (Exception e)
                {
                    Debug.Print(e.Message);
                }
            }
            else
            {
                Debug.Print("C'è già un thread in esecuzione");
            }
        }

        private void connettiServer()
        {
            try
            {
                //clientS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                bool f = true;
                while (f)
                {
                    if (clientS != null)
                    {
                        clientS.Close();
                        Thread.Sleep(1000);
                    }
                    clientS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //Debug.Print("Eseguo connect");
                    clientS.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                    clientS.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 255);
                    clientS.Connect(endPoint);
                    f = clientS.Poll(1000, SelectMode.SelectError);
                }
                Debug.Print("Connessione al server riuscita");
                connesso = true;
                inConnessione = false;
            }
            catch (Exception e)
            {
                inConnessione = false;
                Debug.Print(e.Message);
            }
        }

        public void annullaConnessione()
        {
            if (clientS != null)
            {
                clientS.Close();
                clientS = null;
            }
            this.connesso = false;
        }

        public bool isConnesso()
        {
            return this.connesso;
        }

        // scrittura su socket inviando pacchetti grossi massimo 1460
        // inviamo prima la grandezza della foto(int32) e poi la foto
        public bool ScritturaStream(Byte[] img)
        {
            try
            {
                if (clientS == null)
                {
                    return false;
                }
                int count = sizeof(Int32);
                int i = 0, byteInviati;
                Byte[] data = BitConverter.GetBytes(img.Length);
                while (count > 0)
                {
                    byteInviati = clientS.Send(data, i, count, SocketFlags.None);
                    count -= byteInviati;
                    i += byteInviati;
                }
                i = 0;
                count = img.Length;
                int invia = 512;
                while (count > 0)
                {

                    if (count < 512)
                        invia = count;

                    byteInviati = clientS.Send(img, i, invia, SocketFlags.None);
                    count -= byteInviati;
                    i += byteInviati;
                    Debug.Print("To be sent: " + count);
                    

                    //Thread.Sleep(15);
                }
                return true;
            }
            catch (Exception e)
            {
                if (clientS != null)
                {
                    clientS.Close();
                    clientS = null;
                }
                Debug.Print(e.Message);
                return false;
            }
        }

        // leggo dal server il codice di risposta
        public Int32 LetturaStream()
        {
            Int32 bytesRead = 0;
            Byte[] data = null;
            int count;
            int i = 0;

            try
            {
                if (clientS == null)
                {
                    return -1;
                }

                //leggo il codice di ritorno (int32)
                data = new Byte[sizeof(int)];
                count = sizeof(int);
                while (count > 0)
                {
                    bytesRead = clientS.Receive(data, i, count, SocketFlags.None);
                    i += bytesRead;
                    count -= bytesRead;
                    Debug.Print("leggo: " + bytesRead);
                }

                return BitConverter.ToInt32(data, 0);
            }
            catch (Exception)
            {
                if (clientS != null)
                {
                    clientS.Close();
                    clientS = null;
                }
                return -1;
            }
        }
    }
}
