using System.Text.Json.Serialization;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class TargetSetDiscoverTargetsRequest
    {
        public bool Discover { get; set; }

        public DiscoverFilter[] Filter { get; set; }

        internal class DiscoverFilter
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            public string Type { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            public bool? Exclude { get; set; }
        }
    }
}
