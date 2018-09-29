using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal struct Metric
    {
        [JsonProperty("name")]
        internal string Name { get; set; }
        [JsonProperty("value")]
        internal decimal Value { get; set; }
    }
}