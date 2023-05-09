using System.Text.Json;
using System.Text.Json.Nodes;

namespace PuppeteerSharp.Messaging
{
    internal class ConnectionResponse
    {
        public int? Id { get; set; }

        public ConnectionError Error { get; set; }

        public JsonObject Result { get; set; }

        public string Method { get; set; }

        public JsonElement Params { get; set; }

        public string SessionId { get; set; }
    }
}
