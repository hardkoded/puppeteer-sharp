using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public class PageErrorExceptionProperty
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}