using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using TCPNetworkModule.Json;
using TCPNetworkModule.Message;

namespace Client
{
    public static class MessageHandler
    {
        //Handler on the 'Client' side of the system
        [JsonRoute("$.action", "HeartBeat")]
        public static Task HandleMessage(HeartBeatResponseMessage response)
        {
            Debug.WriteLine($"Received HeartBeatResponseMessage Response: {response?.Result?.Status}, {response?.Id}");
            return Task.CompletedTask;
        }

    }
}
