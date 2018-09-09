using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class EvaluateExceptionInfo
    {
        [JsonProperty("description")]
        internal string Description { get; set; }

        [JsonProperty("value")]
        internal string Value { get; set; }
    }
}