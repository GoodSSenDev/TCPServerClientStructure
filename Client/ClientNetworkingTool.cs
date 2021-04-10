    using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TCPNetworkModule.Json;
using TCPNetworkModule.Message;

namespace Client
{
    public class ClientNetworkingTool
    {
        private readonly JsonClientChannel _channel = new JsonClientChannel();
        private readonly JsonMessageDispatcher _messageDispatcher = new JsonMessageDispatcher();

        private bool isConnected = false;

        public ClientNetworkingTool()
        {
            this._messageDispatcher.Register<HeartBeatResponseMessage>(MessageHandler.HandleMessage);
            
        }

        /// <summary>
        /// Start Connection
        /// Start the HeartBeat loop  
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public async Task ConnectAsync(int port = 5544)
        {
            if(!this.isConnected)
            {
                this._messageDispatcher.Bind(this._channel);
                var endpoint = new IPEndPoint(IPAddress.Loopback, port);

                await this._channel.ConnectAsync(endpoint).ConfigureAwait(false);

                _ = Task.Run(HBMessageLoop);
                this.isConnected = true;
            }
            
        }

        /// <summary>
        /// loop
        /// :Sends the HeartBeat Request Message to inform connection is still alive
        /// </summary>
        /// <returns></returns>
        private async Task HBMessageLoop()
        {
            while(true)
            {
                var hbMessage = new HeartBeatRequestMessage
                {
                    Id = "CLIENT HeartBeat MESSAGE RECEIVED - client is still alive",
                    POSData = new POSData { Id = "POS001" }
                };

                await this._channel.SendAsync(hbMessage).ConfigureAwait(false);
                await Task.Delay(10 * 1000);
            }
        }

    }
}
