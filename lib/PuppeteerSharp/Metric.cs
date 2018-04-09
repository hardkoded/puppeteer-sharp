using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public struct Metric
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public decimal Value { get; set; }
    }
}