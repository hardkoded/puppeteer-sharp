using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public class EvaluationExceptionCallFrame
    {
        [JsonProperty("columnNumber")]
        public int ColumnNumber { get; internal set; }
        [JsonProperty("lineNumber")]
        public int LineNumber { get; internal set; }
        [JsonProperty("url")]
        public string Url { get; internal set; }
        [JsonProperty("functionName")]
        public string FunctionName { get; internal set; }
    }
}