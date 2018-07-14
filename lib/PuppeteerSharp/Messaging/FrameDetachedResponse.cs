using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class FrameDetachedResponse
    {
        [JsonProperty("frameId")]
        public string FrameId { get; set; }
    }
}
