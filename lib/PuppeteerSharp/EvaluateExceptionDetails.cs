using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class EvaluateExceptionDetails
    {
        [JsonProperty("exception")]
        internal EvaluateExceptionInfo Exception { get; set; }
        [JsonProperty("text")]
        internal string Text { get; set; }
        [JsonProperty("stackTrace")]
        internal EvaluateExceptionStackTrace StackTrace { get; set; }
    }
}