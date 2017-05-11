using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication_FakeClient
{
    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint server_endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            UDPNetwork.Client client = new UDPNetwork.Client(server_endpoint);
            //Thread t = new Thread(new ParameterizedThreadStart(new Program().server_function));
            //t.IsBackground = true;
            //t.Start();
            //Thread.Sleep(1000);
            Console.WriteLine("Created client");
            int token = client.AskToken();
            Console.WriteLine("Client received token " + token);

            Bitmap image = new Bitmap("E:\\test4.bmp");

            MemoryStream ms = new MemoryStream();
            image.Save(ms, image.RawFormat);
            byte[] data = ms.ToArray();
            Console.WriteLine("Going to send " + data.Length + " bytes");
            client.SendData(data, token);
            byte[] response = client.ReceiveData(token);
            Console.WriteLine("received response: " + System.Text.Encoding.Default.GetString(response));



            token = client.AskToken();
            Console.WriteLine("Client received token " + token);

            image = new Bitmap("E:\\small.bmp");

            ms = new MemoryStream();
            image.Save(ms, image.RawFormat);
            data = ms.ToArray();
            Console.WriteLine("Going to send " + data.Length + " bytes");
            client.SendData(data, token);
            response = client.ReceiveData(token);
            Console.WriteLine("received response: " + System.Text.Encoding.Default.GetString(response));
        }
    }
}
