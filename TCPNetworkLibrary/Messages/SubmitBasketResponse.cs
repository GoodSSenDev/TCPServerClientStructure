#nullable   enable

using System.Xml.Serialization;

using Newtonsoft.Json;

namespace TCPNetworkModule.Message
{
    public class SubmitBasketResponse : Message
    {
        [JsonProperty( "Result" )]
        public Result? Result { get; set; }

        [JsonProperty( "posData" )]
        public POSData? POSData { get; set; }

        public SubmitBasketResponse( )
        {
            Type = MessageType.Response;
            Action = "SubmitBasket";
        }
    }
}
