using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class LifecycleEventResponse
    {
        [JsonProperty(Constants.FRAME_ID)]
        public string FrameId { get; set; }

        [JsonProperty(Constants.LOADER_ID)]
        public string LoaderId { get; set; }

        [JsonProperty(Constants.NAME)]
        public string Name { get; set; }
    }
}