using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class FramePayload
    {
        [JsonProperty(Constants.ID)]
        internal string Id { get; set; }
        [JsonProperty(Constants.PARENT_ID)]
        internal string ParentId { get; set; }
        [JsonProperty(Constants.NAME)]
        internal string Name { get; set; }
        [JsonProperty(Constants.URL)]
        internal string Url { get; set; }
    }
}