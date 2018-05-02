using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public class PageErrorCallFrame
    {
        [JsonProperty("functionName")]
        public string FunctionName { get; set; }

        [JsonProperty("scriptId")]
        public string ScriptId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("lineNumber")]
        public int LineNumber { get; set; }

        [JsonProperty("columnNumber")]
        public int ColumnNumber { get; set; }
    }
}