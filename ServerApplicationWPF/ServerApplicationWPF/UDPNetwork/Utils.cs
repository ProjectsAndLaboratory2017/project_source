using System;
using System.Net;
using System.Net.Sockets;

namespace ServerApplicationWPF.UDPNetwork
{
    class Utils
    {
        // never want fragmentation: MTU = 1500 usually
        // IP header = 20
        // UDP header = 8
        public static int DGRAM_MAX_SIZE = 1000;
        public static int CHUNK_SIZE = DGRAM_MAX_SIZE - 2 * sizeof(int);
        public static string TOKEN_REQUEST = "give_me_a_token";
        public static string ACK = "ack";
        public static string NACK = "nack";
        public static int RECEIVE_TIMEOUT = 1000; // timeout between net request and response/ack or between token generation and usage
        public static int FRAGMENT_TIMEOUT = 40; // 10 ms for retransimssion
        public static int MAX_RETRY = 100;


        public static byte[] StringToBytes(string s)
        {
            return System.Text.Encoding.UTF8.GetBytes(s);
        }

        public static string BytesToString(byte[] bytes)
        {
            return new string(System.Text.Encoding.UTF8.GetChars(bytes));
        }

        public static byte[] ReceiveFrom(Socket socket, ref EndPoint remoteEndPoint)
        {
            byte[] dgram = new byte[DGRAM_MAX_SIZE];
            int dgram_size = socket.ReceiveFrom(dgram, ref remoteEndPoint);
            byte[] effective_dgram = new byte[dgram_size];
            Array.Copy(dgram, effective_dgram, dgram_size);
            return effective_dgram;
        }

        public static byte[] Receive(Socket socket)
        {
            byte[] dgram = new byte[DGRAM_MAX_SIZE];
            int dgram_size = socket.Receive(dgram);
            byte[] effective_dgram = new byte[dgram_size];
            Array.Copy(dgram, effective_dgram, dgram_size);
            return effective_dgram;
        }
    }
}
