using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class CoverageResponseRange
    {
        [JsonProperty("startOffset")]
        public int StartOffset { get; set; }
        [JsonProperty("endOffset")]
        public int EndOffset { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("styleSheetId")]
        public string StyleSheetId { get; set; }
    }
}
