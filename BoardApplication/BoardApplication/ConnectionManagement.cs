using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;

namespace BoardApplication
{
    class ConnectionManagement
    {
        private IPEndPoint endP;
        private bool connection = false;//false = a process connected to the serve has not been instantiated yet, otherwise it's true.
        private bool connect = false; //The connection between client and server is instantiated.
        private Socket client = null;


        public ConnectionManagement(){
            //IPEndPoint: this method allows you to instantiate the address and port number of the endpoint.
            //IPAddress.Parse("IPAddress")-->This method converts an IP address string to an IPAddress instance.
            //Both methods use the library System.Net.IPEndPoint. This namespace is contained in the .dll file System.dll
           endP = new IPEndPoint(IPAddress.Parse("192.168.1.1"), 8000);
        }

        public void Connect()
        {
            if (!connection && !connect)
            {
                try
                {
                    Debug.Print("Nuovo thread per connettersi al server");
                    connection = true;

                    //ThreadStart: method that executes a thread that has been created previously.
                    ThreadStart threadStart = new ThreadStart(ServerConnection);
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

        private void ServerConnection()
        {
            try
            {
                //clientS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                bool f = true;
                while (f)
                {
                    if (client != null)//if the client is busy with another connection I close it and I create another one.
                    {
                        client.Close();
                        //Thread.Sleep(Millisecond value)-->The number of milliseconds for which the thread is suspended. 
                        //If the value of the millisecondsTimeout argument is zero, the thread relinquishes the remainder 
                        //of its time slice to any thread of equal priority that is ready to run. If there are no other threads 
                        //of equal priority that are ready to run, execution of the current thread is not suspended.
                        Thread.Sleep(1000);
                    }
                    //Using the method Socket you can specify the parameters AddressFamily, type of sockets and type of protocol.
                    client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    //Debug.Print("Eseguo connect");
                    /*SetSocketOption method:
                    -SocketOptionLevel: you specify that the socket options are applied only to a particular type of sockets (TCP,IP etc.) 
                    In our case we usa a TCP connection; therefore you apply this option to TCP sockets.
                    */
                    //client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                    client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 255);
                    client.Connect(endP);//you connect with the server (It shoul be the handshake phase).
                    f = client.Poll(1000, SelectMode.SelectError); //It determines the status of the socket. True if the connections is not available,
                    //otherwise it's false.
                }
                Debug.Print("Connessione al server riuscita");
                connect = true; //The connection between client and server is instantiated.
                connection = false; //Credo che serva per quando viene chiusa la connessione (quindi connesso diventa pari a false) e in 
                //questo modo se la board vuole stabilire una nuova connessione (e quindi un nuovo thread) può farlo.
                //Ciò è dovuto alla presenza dell'if in riga 26. Io personalmente non lo metterei qui. Vedi in seguito dove piazzarlo.
            }
            catch (Exception e)
            {
                connection = false;
                Debug.Print(e.Message);
            }
        }

        public void annullaConnessione()
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }
            this.connect = false;
        }

        public bool isConnected()
        {
            return this.connect;
        }

        // scrittura su socket inviando pacchetti grossi massimo 1460
        // inviamo prima la grandezza della foto(int32) e poi la foto
        public bool WriteStream(Byte[] img)
        {
            try
            {
                if (client == null)
                {
                    return false;
                }
                int count = sizeof(Int32); //dimension of the image.
                int i = 0, byteSent;
                Byte[] data = BitConverter.GetBytes(img.Length);//length of the img in 32-bit.
                while (count > 0)
                {
                    byteSent = client.Send(data, i, count, SocketFlags.Partial);//Firstly I send the dimension.
                    count -= byteSent;
                    i += byteSent;
                }
                i = 0;
                count = img.Length;
                int invia = 20000;
                while (count > 0)
                {

                    if (count < 20000)
                        invia = count;

                    byteSent = client.Send(img, i, invia, SocketFlags.None);
                    count -= byteSent;
                    i += byteSent;
                    Debug.Print("To be sent: " + count);


                    //Thread.Sleep(15);
                }
                return true; //this return value if all the sending have succesful
            }
            catch (Exception e)
            {
                if (client != null)
                {
                    client.Close();
                    client = null;
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
                if (client == null)
                {
                    return -1;
                }

                //leggo il codice di ritorno (int32)
                data = new Byte[sizeof(int)];
                count = sizeof(int);
                while (count > 0)
                {
                    bytesRead = client.Receive(data, i, count, SocketFlags.None);
                    i += bytesRead;
                    count -= bytesRead;
                    Debug.Print("I read: " + bytesRead);
                }

                return BitConverter.ToInt32(data, 0);
            }
            catch (Exception)
            {
                if (client != null)
                {
                    client.Close();
                    client = null;
                }
                return -1;
            }
        }

        public void StopConnection()
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }
            this.connect = false;
        }

        public bool isConnesso()
        {
            return this.connect;
        }

    }


}
