using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TCPNetworkModule.Json;
using TCPNetworkModule.Messages;

namespace Server
{
    public static class MessageHandler
    {
        //Handler on the 'Server' side of the system
        [JsonRoute("$.action", "HeartBeat")]
        public static Task<HeartBeatResponseMessage> HandleMessage(HeartBeatRequestMessage request)
        {
            Received(request);
            Console.WriteLine("HeartBeatRequestMessage ");
            var response = new HeartBeatResponseMessage
            {
                Id = request.Id,
                POSData = request.POSData,
                Result = new Result { Status = Status.Success }
            };
            Sending(response);
            return Task.FromResult(response);
        }


        static void Received<T>(T msg) where T : Message
            => Console.WriteLine($"Received {typeof(T).Name}: Action[ {msg.Action} ], Id[ {msg.Id} ]");

        static void Sending<T>(T msg) where T : Message
            => Console.WriteLine($"Received {typeof(T).Name}: Action[ {msg.Action} ], Id[ {msg.Id} ]");
    }
}
