using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class FramePayload
    {
        [JsonProperty("id")]
        internal string Id { get; set; }
        [JsonProperty("parentId")]
        internal string ParentId { get; set; }
        [JsonProperty("name")]
        internal string Name { get; set; }
        [JsonProperty("url")]
        internal string Url { get; set; }
    }
}