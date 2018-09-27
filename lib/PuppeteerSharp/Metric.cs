using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal struct Metric
    {
        [JsonProperty(Constants.NAME)]
        internal string Name { get; set; }
        [JsonProperty(Constants.VALUE)]
        internal decimal Value { get; set; }
    }
}