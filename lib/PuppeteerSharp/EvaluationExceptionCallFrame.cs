using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class EvaluationExceptionCallFrame
    {
        [JsonProperty("columnNumber")]
        internal int ColumnNumber { get; set; }
        [JsonProperty("lineNumber")]
        internal int LineNumber { get; set; }
        [JsonProperty("url")]
        internal string Url { get; set; }
        [JsonProperty("functionName")]
        internal string FunctionName { get; set; }
    }
}