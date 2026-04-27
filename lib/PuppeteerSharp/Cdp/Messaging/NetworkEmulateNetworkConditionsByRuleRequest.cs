namespace PuppeteerSharp.Cdp.Messaging
{
    internal class NetworkEmulateNetworkConditionsByRuleRequest
    {
        public MatchedNetworkCondition[] MatchedNetworkConditions { get; set; }

        public bool Offline { get; set; }
    }
}
