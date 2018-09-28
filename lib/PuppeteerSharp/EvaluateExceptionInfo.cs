using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class EvaluateExceptionInfo
    {
        [JsonProperty(Constants.DESCRIPTION)]
        internal string Description { get; set; }

        [JsonProperty(Constants.VALUE)]
        internal string Value { get; set; }
    }
}