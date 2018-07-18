using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class NavigatedWithinDocumentResponse
    {
        [JsonProperty("frameId")]
        public string FrameId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
