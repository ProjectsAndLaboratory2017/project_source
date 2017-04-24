using System;
using Microsoft.SPOT;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace HelloLED
{
    class GestioneConnessione
    {
        private Socket clientS = null;
        private bool inConnessione = false; //false = a process connected to the serve has not been instantiated yet, otherwise it's true.
        private bool connesso = false; //
        private IPEndPoint endPoint;

        public GestioneConnessione()
        {	
			//IPEndPoint: this method allows you to instantiate the address and port number of the endpoint.
			//IPAddress.Parse("IPAddress")-->This method converts an IP address string to an IPAddress instance.
			//Both methods use the library System.Net.IPEndPoint. This namespace is contained in the .dll file System.dll
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
	
					//ThreadStart: method that executes a thread that has been created previously.
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
                    if (clientS != null)//if the client is busy with another connection I close it and I create another one.
                    {
                        clientS.Close();
						//Thread.Sleep(Millisecond value)-->The number of milliseconds for which the thread is suspended. 
						//If the value of the millisecondsTimeout argument is zero, the thread relinquishes the remainder 
						//of its time slice to any thread of equal priority that is ready to run. If there are no other threads 
						//of equal priority that are ready to run, execution of the current thread is not suspended.
                        Thread.Sleep(1000);
                    }
					//Using the method Socket you can specify the parameters AddressFamily, type of sockets and type of protocol.
                    clientS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //Debug.Print("Eseguo connect");
					/*SetSocketOption method:
					-SocketOptionLevel: you specify that the socket options are applied only to a particular type of sockets (TCP,IP etc.) 
					In our case we usa a TCP connection; therefore you apply this option to TCP sockets.
					*/
                    clientS.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                    clientS.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 255);
                    clientS.Connect(endPoint);//you connect with the server (It shoul be the handshake phase).
                    f = clientS.Poll(1000, SelectMode.SelectError); //It determines the status of the socket. True if the connections is not available,
																	//otherwise it's false.
                }
                Debug.Print("Connessione al server riuscita");
                connesso = true; //The connection between client and server is instantiated.
                inConnessione = false; //Credo che serva per quando viene chiusa la connessione (quindi connesso diventa pari a false) e in 
									   //questo modo se la board vuole stabilire una nuova connessione (e quindi un nuovo thread) può farlo.
									   //Ciò è dovuto alla presenza dell'if in riga 26. Io personalmente non lo metterei qui. Vedi in seguito dove piazzarlo.
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
                int count = sizeof(Int32); //dimension of the image.
                int i = 0, byteInviati;
                Byte[] data = BitConverter.GetBytes(img.Length);//length of the img in 32-bit.
                while (count > 0)
                {
                    byteInviati = clientS.Send(data, i, count, SocketFlags.None);//Firstly I send the dimension.
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
                return true; //this return value if all the sending have succesful
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
