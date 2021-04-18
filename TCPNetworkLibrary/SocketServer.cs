using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TCPNetworkModule
{
    public abstract class SocketServer<TChannelType, TProtocol, TSerializedDataType, TMessageDispatcher>
        where TChannelType : Channel<TProtocol, TSerializedDataType>, new()
        where TProtocol : Protocol<TSerializedDataType>, new()
        where TSerializedDataType : class, new()
        where TMessageDispatcher : MessageDispatcher<TSerializedDataType>, new()
    {
        readonly ChannelManager _channelManager;
        readonly TMessageDispatcher _messageDispatcher;


        public SocketServer(TMessageDispatcher messageDispatcher)
        {
            _messageDispatcher = messageDispatcher;
            _channelManager = new ChannelManager(() => {
                var channel = CreateChannel();
                _messageDispatcher.Bind(channel);
                return channel;
            });
        }

        public SocketServer()
        {
            _messageDispatcher = new TMessageDispatcher();
            _channelManager = new ChannelManager(() => {
                var channel = CreateChannel();
                _messageDispatcher.Bind(channel);
                return channel;
            });
        }

        public void Start(int port = 5544)
        {
            var endPoint = new IPEndPoint(IPAddress.Loopback, port);

            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endPoint);
            socket.Listen(128);
            _ = Task.Run(() => RunAsync(socket));

        }

        private async Task RunAsync(Socket socket)
        {

            do
            {
                var clientSocket = await Task.Factory.FromAsync(
                    new Func<AsyncCallback, object, IAsyncResult>(socket.BeginAccept),
                    new Func<IAsyncResult, Socket>(socket.EndAccept),
                    null).ConfigureAwait(false);

                Console.WriteLine("SERVER :: CLIENT CONNECTION REQUEST");

                _channelManager.Accept(clientSocket);

                Console.WriteLine("SERVER :: CLIENT CONNECTED");

            } while (true);
        }


        protected virtual TChannelType CreateChannel() => new TChannelType();

    }
}
