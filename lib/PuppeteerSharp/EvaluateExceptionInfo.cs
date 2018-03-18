using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public class EvaluateExceptionInfo
    {
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}