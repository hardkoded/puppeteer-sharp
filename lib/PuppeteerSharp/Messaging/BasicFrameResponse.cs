using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class BasicFrameResponse
    {
        [JsonProperty(Constants.FRAME_ID)]
        public string FrameId { get; set; }
    }
}