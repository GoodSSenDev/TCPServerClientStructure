using Newtonsoft.Json.Linq;

namespace TCPNetworkModule.Json
{
    public class JsonSocketServer
        : SocketServer<JsonChannel, JsonMessageProtocol, JObject, JsonMessageDispatcher>
    {
        public JsonSocketServer(JsonMessageDispatcher messageDispatcher) : base(messageDispatcher)
        {
        }
    }
}
