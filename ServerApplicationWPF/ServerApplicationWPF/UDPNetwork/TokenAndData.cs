using System;

namespace ServerApplicationWPF.UDPNetwork
{
    public class TokenAndData
    {
        public byte[] Serialized { get; private set; }
        public int Token {get; private set; }
        public byte[] Data { get; private set; }

        public TokenAndData(byte[] tokenAndData)
        {
            Serialized = tokenAndData;
            Data = new byte[tokenAndData.Length - sizeof(int)];
            Token = BitConverter.ToInt32(tokenAndData, 0);
            Array.Copy(tokenAndData, sizeof(int), Data, 0, Data.Length);
        }

        public TokenAndData(int token, byte[] data)
        {
            Token = (int)token;
            Data = data;
            Serialized = new byte[sizeof(uint) + data.Length];
            Array.Copy(BitConverter.GetBytes(Token), 0, Serialized, 0, sizeof(int));
            Array.Copy(Data, 0, Serialized, sizeof(int), data.Length);
            
        }
    }
}
