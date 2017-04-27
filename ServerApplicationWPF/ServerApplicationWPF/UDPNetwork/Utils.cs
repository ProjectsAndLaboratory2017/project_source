using System;

namespace ServerApplicationWPF.UDPNetwork
{
    class Utils
    {
        // never want fragmentation: MTU = 1500 usually
        // IP header = 20
        // UDP header = 8
        public static int CHUNK_SIZE = 1472 - sizeof(int);
        public static string TOKEN_REQUEST = "give_me_a_token";
        public static string ACK = "ack";
        public static int RECEIVE_TIMEOUT = 20000; // timeout between net request and response/ack or between token generation and usage

        public static byte[] StringToBytes(string s)
        {
            return System.Text.Encoding.UTF8.GetBytes(s);
        }

        public static string BytesToString(byte[] bytes)
        {
            return new string(System.Text.Encoding.UTF8.GetChars(bytes));
        }
    }
}
