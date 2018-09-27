using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class LifecycleEventResponse
    {
        [JsonProperty("frameId")]
        public string FrameId { get; set; }

        [JsonProperty("loaderId")]
        public string LoaderId { get; set; }

        [JsonProperty(Constants.NAME)]
        public string Name { get; set; }
    }
}