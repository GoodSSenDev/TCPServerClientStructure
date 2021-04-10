#nullable   enable

using System.Xml.Serialization;

using Newtonsoft.Json;

namespace TCPNetworkModule.Message
{ 
    public class SubmitBasketRequest : Message
    {

        [JsonProperty( "posData" )]
        public POSData? POSData { get; set; }

        public SubmitBasketRequest( )
        {
            Type = MessageType.Request;
            Action = "SubmitBasket";
        }
    }
}
