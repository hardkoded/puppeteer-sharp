using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class CoverageResponseRange
    {
        [JsonProperty(Constants.START_OFFSET)]
        public int StartOffset { get; set; }
        [JsonProperty(Constants.END_OFFSET)]
        public int EndOffset { get; set; }
        [JsonProperty(Constants.COUNT)]
        public int Count { get; set; }
        [JsonProperty(Constants.STYLE_SHEET_ID)]
        public string StyleSheetId { get; set; }
    }
}
