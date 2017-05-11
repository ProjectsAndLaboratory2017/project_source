using System;

namespace BoardApplication
{
    public class TokenAndData
    {
        public byte[] Serialized { get; private set; }
        public int Token { get; private set; }
        public int SequenceNumber { get; private set; }
        public byte[] Data { get; private set; }

        public TokenAndData(byte[] tokenAndData)
        {
            Serialized = tokenAndData;
            Data = new byte[tokenAndData.Length - 2 * sizeof(int)];
            Token = BitConverter.ToInt32(tokenAndData, 0);
            SequenceNumber = BitConverter.ToInt32(tokenAndData, sizeof(int));
            Array.Copy(tokenAndData, 2 * sizeof(int), Data, 0, Data.Length);
        }

        public TokenAndData(int token, int sequenceNumber, byte[] data)
        {
            Token = token;
            SequenceNumber = sequenceNumber;
            Data = data;
            Serialized = new byte[2 * sizeof(uint) + data.Length];
            Array.Copy(BitConverter.GetBytes(Token), 0, Serialized, 0, sizeof(int));
            Array.Copy(BitConverter.GetBytes(SequenceNumber), 0, Serialized, sizeof(int), sizeof(int));
            Array.Copy(Data, 0, Serialized, 2 * sizeof(int), data.Length);

        }
    }
}
