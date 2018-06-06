using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class RequestWillBeSentResponse
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("request")]
        public Request Request { get; set; }

        [JsonProperty("redirectResponse", NullValueHandling = NullValueHandling.Ignore)]
        public Response RedirectResponse { get; set; }

        [JsonProperty("type")]
        public ResourceType Type { get; set; }

        [JsonProperty("frameId")]
        public string FrameId { get; set; }
    }
}
