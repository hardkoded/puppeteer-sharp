using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class FramePayload
    {
        [JsonProperty(Constants.ID)]
        internal string Id { get; set; }
        [JsonProperty("parentId")]
        internal string ParentId { get; set; }
        [JsonProperty(Constants.NAME)]
        internal string Name { get; set; }
        [JsonProperty("url")]
        internal string Url { get; set; }
    }
}