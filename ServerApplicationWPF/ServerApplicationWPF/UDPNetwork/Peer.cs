using System;
using System.Net;
using System.Net.Sockets;

namespace ServerApplicationWPF.UDPNetwork
{
    public class Peer
    {
        protected UdpClient udpClient;
        protected IPEndPoint remoteEndpoint;

        protected Peer()
        {

        }
        public void SendData(byte[] data, int token)
        {
            int toSend = data.Length;
            // send the length
            TokenAndData length_dgram = new TokenAndData(token, BitConverter.GetBytes(data.Length));
            udpClient.Send(length_dgram.Serialized, length_dgram.Serialized.Length);
            int start_offset = 0;
            while (toSend > 0)
            {
                int send_now = Math.Min(toSend, Utils.CHUNK_SIZE);
                byte[] buff = new byte[send_now];
                int sent = 0;
                Array.Copy(data, start_offset, buff, 0, send_now);
                // create the datagram content: token followed by a chunk of data
                TokenAndData dgram = new TokenAndData(token, buff);
                sent = udpClient.Send(dgram.Serialized, dgram.Serialized.Length);
                toSend -= send_now;
                start_offset += send_now;
            }
            // now wait for the ACK a limited time
            udpClient.Client.ReceiveTimeout = Utils.RECEIVE_TIMEOUT;
            byte[] response = udpClient.Receive(ref remoteEndpoint);
            TokenAndData response_parsed = new TokenAndData(response);
            if (response_parsed.Token != token)
            {
                throw new Exception("The server answered with another token: " + response_parsed.Token);
            }
            string ack_string = Utils.BytesToString(response_parsed.Data);
            if (ack_string != Utils.ACK)
            {
                throw new Exception("The server was expected to ack instead replied with " + ack_string);
            }
        }

        public byte[] ReceiveData(int token)
        {
            // wait for a limited time because a token has limited lifespan
            udpClient.Client.ReceiveTimeout = Utils.RECEIVE_TIMEOUT;
            // read the length
            byte[] dgram = udpClient.Receive(ref remoteEndpoint);
            TokenAndData dgram_parsed = new TokenAndData(dgram);
            int toRead = BitConverter.ToInt32(dgram_parsed.Data, 0);
            if (dgram_parsed.Token != token)
            {
                throw new Exception("Wrong token " + dgram_parsed.Token);
            }
            byte[] result = new byte[toRead];
            int start_offset = 0;
            while (toRead > 0)
            {
                byte[] dgram_data = udpClient.Receive(ref remoteEndpoint);
                TokenAndData data_parsed = new TokenAndData(dgram_data);
                if (data_parsed.Token != token)
                {
                    throw new Exception("Wrong token " + dgram_parsed.Token);
                }
                Array.Copy(data_parsed.Data, 0, result, start_offset, data_parsed.Data.Length);
                start_offset += data_parsed.Data.Length;
                toRead -= data_parsed.Data.Length;
            }
            // now send the ACK
            TokenAndData ack = new TokenAndData(token, Utils.StringToBytes("ack"));
            udpClient.Send(ack.Serialized, ack.Serialized.Length);

            return result;
        }
    }
}
