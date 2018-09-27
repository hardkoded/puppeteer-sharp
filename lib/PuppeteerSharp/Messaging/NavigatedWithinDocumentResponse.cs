using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class NavigatedWithinDocumentResponse
    {
        [JsonProperty("frameId")]
        public string FrameId { get; set; }

        [JsonProperty(Constants.URL)]
        public string Url { get; set; }
    }
}
