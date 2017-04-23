﻿using System;
using System.Collections.Generic;
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

            int size = 1000000;

            byte[] data = new byte[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = (byte)(i / 10);
            }
            client.SendData(data, token);
            byte[] response = client.ReceiveData(token);
            Console.WriteLine("received response: " + System.Text.Encoding.Default.GetString(response));
        }

        void server_function(object obj)
        {
            UDPNetwork.Server server = new UDPNetwork.Server(8000);
            Console.WriteLine("Server alive");
            int token = server.Accept();
            Console.WriteLine("Connection accepted. Gave token " + token);
            byte[] data = server.ReceiveData(token);
            Console.WriteLine("received data: " + data.Length);
            server.SendData(System.Text.Encoding.Default.GetBytes("ciao"), token);
        }
    }
}