using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TCPNetworkModule.Json;
using TCPNetworkModule.Message;

namespace Server
{
    public class TCPSocketServer
    {
        readonly JsonMessageDispatcher _messageDispatcher = new JsonMessageDispatcher();

        public TCPSocketServer()
        {
            //TODO: Register Messages
            _messageDispatcher.Register<HeartBeatRequestMessage, HeartBeatResponseMessage>(MessageHandler.HandleMessage);
        }

        /// <summary>
        /// start the server
        /// start reseponse loop
        /// </summary>
        /// <param name="port"></param>
        public void Start(int port = 5544)
        {
            Console.WriteLine("SERVER IS STARTING");

            var endPoint = new IPEndPoint(IPAddress.Loopback, port);

            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endPoint);
            socket.Listen(128); //how much log will save 

            _ = Task.Run(() => DoResponse(socket));

            Console.WriteLine("SERVER IS READY");
        }

        /// <summary>
        /// This method basically do the response when message arrives.
        /// Create a socket that is connected to the client
        /// attach the socket to the channel
        /// bind the channel and dispatcher so arrived message can go directly a message dispatcher.
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        private async Task DoResponse(Socket socket)
        {
            do
            {
                var clientSocket = await Task.Factory.FromAsync(
                    new Func<AsyncCallback, object, IAsyncResult>(socket.BeginAccept),
                    new Func<IAsyncResult, Socket>(socket.EndAccept),
                    null).ConfigureAwait(false);

                Console.WriteLine("ECHO SERVER :: CLIENT CONNECTED");

                var channel = new JsonChannel();

                _messageDispatcher.Bind(channel);

                channel.Attach(clientSocket);

                while (true) { }

            } while (true);
        }

    }
}
