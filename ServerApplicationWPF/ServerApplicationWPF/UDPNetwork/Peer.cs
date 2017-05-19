using System;
using System.Net;
using System.Net.Sockets;

namespace ServerApplicationWPF.UDPNetwork
{
    public class Peer
    {
        protected Socket socket;
        protected EndPoint remoteEndpoint;

        protected Peer()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }
        public void SendData(byte[] data, int token)
        {
            int toSend = data.Length;
            int sequenceNumber = 0;
            // send the length
            TokenAndData length_dgram = new TokenAndData(token, sequenceNumber, BitConverter.GetBytes(data.Length));
            socket.SendTo(length_dgram.Serialized, remoteEndpoint);
            int start_offset = 0;
            /*
            // wait the ack for the length
            byte[] datagram_ack_length = Utils.ReceiveFrom(socket, ref remoteEndpoint);
            TokenAndData response_parsed = new TokenAndData(datagram);
            */
            sequenceNumber++;
            while (toSend > 0)
            {
                int send_now = Math.Min(toSend, Utils.CHUNK_SIZE);
                byte[] buff = new byte[send_now];
                int sent = 0;
                Array.Copy(data, start_offset, buff, 0, send_now);
                toSend -= send_now;
                start_offset += send_now;
                string ack_string = Utils.NACK;
                int retry = Utils.MAX_RETRY;
                do
                {
                    try
                    {
                        // create the datagram content: token followed by a chunk of data
                        TokenAndData dgram = new TokenAndData(token, sequenceNumber, buff);
                        sent = socket.SendTo(dgram.Serialized, remoteEndpoint);

                        // now wait for the ACK a limited time
                        socket.ReceiveTimeout = Utils.FRAGMENT_TIMEOUT;
                        byte[] datagram = Utils.ReceiveFrom(socket, ref remoteEndpoint);
                        TokenAndData response_parsed = new TokenAndData(datagram);
                        if (response_parsed.Token != token)
                        {
                            retry = 0;
                            throw new Exception("The server answered with another token: " + response_parsed.Token);
                        }
                        if (response_parsed.SequenceNumber != sequenceNumber)
                        {
                            if (response_parsed.SequenceNumber < sequenceNumber)
                            {
                                // already received
                                continue;
                            }
                            throw new Exception("The server answered with a wrong sequence number " + response_parsed.SequenceNumber);
                        }
                        ack_string = Utils.BytesToString(response_parsed.Data);
                    }
                    catch (SocketException e)
                    {
                        //
                    }
                    //retry--;
                } while (ack_string != Utils.ACK && retry > 0);
                if (retry <= 0)
                {
                    throw new Exception("Max number of trials reached");
                }
                // put again retry to max value because transmission succeeded
                retry = Utils.MAX_RETRY;
                sequenceNumber++;
            }
            
        }

        public byte[] ReceiveData(int token)
        {
            // wait for a limited time because a token has limited lifespan
            socket.ReceiveTimeout = Utils.RECEIVE_TIMEOUT;
            // read the length
            byte[] datagram = Utils.ReceiveFrom(socket, ref remoteEndpoint);
            TokenAndData dgram_parsed = new TokenAndData(datagram);
            int toRead = BitConverter.ToInt32(dgram_parsed.Data, 0);
            if (dgram_parsed.Token != token)
            {
                throw new Exception("Wrong token " + dgram_parsed.Token);
            }
            int sequenceNumber = 0;
            byte[] result = new byte[toRead];
            int start_offset = 0;
            int n_packets = 0;
            int toread_orig = toRead;
            sequenceNumber++;
            // switch to aggressive mode
            socket.ReceiveTimeout = Utils.FRAGMENT_TIMEOUT;
            int retry = Utils.MAX_RETRY;
            while (toRead > 0)
            {
                try
                { 
                    datagram = Utils.ReceiveFrom(socket, ref remoteEndpoint);
                    n_packets++;
                    TokenAndData data_parsed = new TokenAndData(datagram);
                    if (data_parsed.Token != token)
                    {
                        retry = 0;
                        throw new Exception("Wrong token: " + dgram_parsed.Token);
                    }
                    if (data_parsed.SequenceNumber != sequenceNumber)
                    {
                        if (data_parsed.SequenceNumber < sequenceNumber)
                        {
                            // already received
                            // send anyway ack (maybe lost it before)
                            TokenAndData nack = new TokenAndData(token, data_parsed.SequenceNumber, Utils.StringToBytes(Utils.ACK));
                            socket.SendTo(nack.Serialized, remoteEndpoint);
                            continue;
                        }
                        throw new Exception("Wrong sequence number " + dgram_parsed.SequenceNumber);
                    }
                    Array.Copy(data_parsed.Data, 0, result, start_offset, data_parsed.Data.Length);
                    start_offset += data_parsed.Data.Length;
                    toRead -= data_parsed.Data.Length;
                    // now send the ACK
                    TokenAndData ack = new TokenAndData(token, sequenceNumber, Utils.StringToBytes(Utils.ACK));
                    socket.SendTo(ack.Serialized, remoteEndpoint);
                    sequenceNumber++;
                }
                catch (Exception e)
                {
                    // some exception (wrong token or timeout expired)
                    // ask again for the same fragment
                    //retry--;
                    TokenAndData nack = new TokenAndData(token, sequenceNumber, Utils.StringToBytes(Utils.NACK));
                    socket.SendTo(nack.Serialized, remoteEndpoint);
                }
                if (retry <= 0)
                {
                    throw new Exception("Max number of trials reached");
                }
            }
            
            return result;
        }
    }
}
