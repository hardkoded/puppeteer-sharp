namespace PuppeteerSharp.Cdp.Messaging
{
    internal class TargetSetDiscoverTargetsRequest
    {
        public bool Discover { get; set; }

        public DiscoverFilter[] Filter { get; set; }

        internal class DiscoverFilter
        {
            public string Type { get; set; }

            public bool? Exclude { get; set; }
        }
    }
}
