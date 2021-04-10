
using Newtonsoft.Json.Linq;
using TCPNetworkModule;

namespace TCPNetworkModule.Json
{
    public class JsonChannel : Channel<JsonMessageProtocol, JObject> { }

    public class JsonClientChannel : ClientChannel<JsonMessageProtocol, JObject> { }

}
