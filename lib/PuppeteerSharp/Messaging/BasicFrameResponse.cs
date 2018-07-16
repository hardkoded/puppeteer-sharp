using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class BasicFrameResponse
    {
        [JsonProperty("frameId")]
        public string FrameId { get; set; }
    }
}