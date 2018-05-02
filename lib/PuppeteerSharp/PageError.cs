using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public class PageError
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("lineNumber")]
        public int LineNumber { get; set; }

        [JsonProperty("columnNumber")]
        public string ColumnNumber { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("exception")]
        public PageErrorException Exception { get; set; }

        [JsonProperty("stackTrace")]
        public PageErrorStackTrace StackTrace { get; set; }
    }
}