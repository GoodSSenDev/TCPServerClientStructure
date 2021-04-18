using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPNetworkModule
{
    public class ClientChannel<TProtocol, TSerializedDataType> : Channel<TProtocol, TSerializedDataType>
        where TProtocol : Protocol<TSerializedDataType>, new()
    {

        public async Task ConnectAsync(IPEndPoint endPoint)
        {
            try
            {
                var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(endPoint).ConfigureAwait(false);
                Attach(socket);
            }
            catch (Exception _e)
            {
                Debug.WriteLine($"Exception in ClientChannel::ConnectAsync {_e}");
            }
        }
    }
}
