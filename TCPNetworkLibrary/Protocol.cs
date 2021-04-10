using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPNetworkModule
{
    public abstract class Protocol<TSerializedDataType>
    {

        const int HEADER_SIZE = 4;//this will be used to indicate length of the body

        public async Task<TSerializedDataType> ReceiveAsync(NetworkStream networkStream)
        {
            var bodyLength = await ReadHeader(networkStream).ConfigureAwait(false);
            AssertValidMessageLength(bodyLength);
            return await ReadBody(networkStream, bodyLength).ConfigureAwait(false);
        }

        public async Task SendAsync<TMessage>(NetworkStream networkStream, TMessage message)
        {
            var (header, body) = Encode<TMessage>(message);

            var output = new byte[header.Length + body.Length];
            Buffer.BlockCopy(src: header, 0, dst: output, 0, header.Length);
            Buffer.BlockCopy(src: body, 0, dst: output, header.Length, body.Length);
            await networkStream.WriteAsync(output, 0, output.Length);
        }

        async Task<int> ReadHeader(NetworkStream networkStream)
        {
            var headerBytes = await ReadAsync(networkStream, HEADER_SIZE).ConfigureAwait(false);
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerBytes));
        }

        async Task<TSerializedDataType> ReadBody(NetworkStream networkStream, int bodyLength)
        {
            var bodyBytes = await ReadAsync(networkStream, bodyLength).ConfigureAwait(false);
            return Decode(bodyBytes);
        }

        async Task<byte[]> ReadAsync(NetworkStream networkStream, int bytesToRead)
        {
            var buffer = new byte[bytesToRead];
            var bytesRead = 0;
            while (bytesRead < bytesToRead)
            {
                var bytesReceived = await networkStream.ReadAsync(buffer, bytesRead, (bytesToRead - bytesRead)).ConfigureAwait(false);
                if (bytesReceived == 0)
                    throw new Exception("Socket Closed");
                bytesRead += bytesReceived;
            }
            return buffer;
        }

        protected (byte[] header, byte[] body) Encode<TMessage>(TMessage message)
        {
            var bodyBytes = EncodeBody<TMessage>(message);
            var headerBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(bodyBytes.Length));
            return (headerBytes, bodyBytes);
        }

        protected abstract TSerializedDataType Decode(byte[] message);

        protected abstract byte[] EncodeBody<TMessage>(TMessage message);

        protected virtual void AssertValidMessageLength(int messageLength)
        {
            if (messageLength < 1)
                throw new ArgumentOutOfRangeException("Invalid Message Length");
        }
    }
}
