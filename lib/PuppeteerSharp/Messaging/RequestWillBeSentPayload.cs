using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class RequestWillBeSentPayload
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("loaderId")]
        public string LoaderId { get; set; }

        [JsonProperty("request")]
        public Payload Request { get; set; }

        [JsonProperty("redirectResponse", NullValueHandling = NullValueHandling.Ignore)]
        public ResponsePayload RedirectResponse { get; set; }

        [JsonProperty(Constants.TYPE)]
        public ResourceType Type { get; set; }

        [JsonProperty("frameId")]
        public string FrameId { get; set; }
    }
}
