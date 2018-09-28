using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class EvaluationExceptionCallFrame
    {
        [JsonProperty(Constants.COLUMN_NUMBER)]
        internal int ColumnNumber { get; set; }
        [JsonProperty(Constants.LINE_NUMBER)]
        internal int LineNumber { get; set; }
        [JsonProperty(Constants.URL)]
        internal string Url { get; set; }
        [JsonProperty(Constants.FUNCTION_NAME)]
        internal string FunctionName { get; set; }
    }
}