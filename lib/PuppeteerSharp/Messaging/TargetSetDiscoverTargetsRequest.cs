using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class TargetSetDiscoverTargetsRequest
    {
        public bool Discover { get; set; }

        public DiscoverFilter[] Filter { get; set; }

        internal class DiscoverFilter
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Type { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public bool? Exclude { get; set;  }
        }
    }
}
