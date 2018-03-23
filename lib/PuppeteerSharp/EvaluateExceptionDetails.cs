using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public class EvaluateExceptionDetails
    {
        [JsonProperty("exception")]
        public EvaluateExceptionInfo Exception { get; internal set; }
        [JsonProperty("text")]
        public string Text { get; internal set; }
        [JsonProperty("stackTrace")]
        public EvaluateExceptionStackTrace StackTrace { get; internal set; }
    }
}