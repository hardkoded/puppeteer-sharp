using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class RequestWillBeSentPayload
    {
        public string RequestId { get; set; }
        public string LoaderId { get; set; }
        public Payload Request { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ResponsePayload RedirectResponse { get; set; }
        public ResourceType Type { get; set; }
        public string FrameId { get; set; }
    }
}
