using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class CSSStopRuleUsageTrackingResponse
    {
        [JsonProperty("ruleUsage")]
        internal CSSStopRuleUsageTrackingRuleUsage[] RuleUsage { get; set; }

        internal class CSSStopRuleUsageTrackingRuleUsage
        {
            [JsonProperty("styleSheetId")]
            public string StyleSheetId { get; set; }
            [JsonProperty("startOffset")]
            public int StartOffset { get; set; }
            [JsonProperty("endOffset")]
            public int EndOffset { get; set; }
            [JsonProperty("used")]
            public bool Used { get; set; }
        }
    }
}
