using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class RequestWillBeSentPayload
    {
        [JsonProperty(Constants.REQUEST_ID)]
        public string RequestId { get; set; }

        [JsonProperty(Constants.LOADER_ID)]
        public string LoaderId { get; set; }

        [JsonProperty(Constants.REQUEST)]
        public Payload Request { get; set; }

        [JsonProperty(Constants.REDIRECT_RESPONSE, NullValueHandling = NullValueHandling.Ignore)]
        public ResponsePayload RedirectResponse { get; set; }

        [JsonProperty(Constants.TYPE)]
        public ResourceType Type { get; set; }

        [JsonProperty(Constants.FRAME_ID)]
        public string FrameId { get; set; }
    }
}
