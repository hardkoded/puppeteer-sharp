using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public class FramePayload
    {
        [JsonProperty("id")]
        public string Id { get; internal set; }
        [JsonProperty("parentId")]
        public string ParentId { get; internal set; }
        [JsonProperty("name")]
        public string Name { get; internal set; }
        [JsonProperty("url")]
        public string Url { get; internal set; }
    }
}