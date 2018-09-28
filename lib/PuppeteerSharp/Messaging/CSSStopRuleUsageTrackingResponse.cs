using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class CSSStopRuleUsageTrackingResponse
    {
        [JsonProperty(Constants.RULE_USAGE)]
        internal CSSStopRuleUsageTrackingRuleUsage[] RuleUsage { get; set; }

        internal class CSSStopRuleUsageTrackingRuleUsage
        {
            [JsonProperty(Constants.STYLE_SHEET_ID)]
            public string StyleSheetId { get; set; }
            [JsonProperty(Constants.START_OFFSET)]
            public int StartOffset { get; set; }
            [JsonProperty(Constants.END_OFFSET)]
            public int EndOffset { get; set; }
            [JsonProperty(Constants.USED)]
            public bool Used { get; set; }
        }
    }
}
