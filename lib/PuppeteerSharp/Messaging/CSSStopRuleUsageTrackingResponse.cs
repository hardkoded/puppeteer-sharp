namespace PuppeteerSharp.Messaging
{
    internal class CSSStopRuleUsageTrackingResponse
    {
        public CSSStopRuleUsageTrackingRuleUsage[] RuleUsage { get; set; }

        internal class CSSStopRuleUsageTrackingRuleUsage
        {
            public string StyleSheetId { get; set; }
            public int StartOffset { get; set; }
            public int EndOffset { get; set; }
            public bool Used { get; set; }
        }
    }
}
