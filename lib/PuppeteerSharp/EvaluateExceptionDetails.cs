using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class EvaluateExceptionDetails
    {
        [JsonProperty(Constants.EXCEPTION)]
        internal EvaluateExceptionInfo Exception { get; set; }
        [JsonProperty(Constants.TEXT)]
        internal string Text { get; set; }
        [JsonProperty(Constants.STACK_TRACE)]
        internal EvaluateExceptionStackTrace StackTrace { get; set; }
    }
}