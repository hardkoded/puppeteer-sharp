using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public class PageErrorException
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("subtype")]
        public string Subtype { get; set; }

        [JsonProperty("className")]
        public string ClassName { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("preview")]
        public PageErrorPreview Preview { get; set; }
    }
}