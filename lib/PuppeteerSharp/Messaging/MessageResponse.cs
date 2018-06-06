using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class MessageResponse
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("request")]
        public Request Request { get; set; }
    }
}
