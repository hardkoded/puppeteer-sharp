using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class ResponseReceivedResponse
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("response")]
        public ResponseData Response { get; set; }
    }
}
