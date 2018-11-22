namespace PuppeteerSharp.Messaging
{
    internal class GetLayoutMetricsResponse
    {
        public GetLayoutMetricsLayoutViewport LayoutViewport { get; set; }

        internal class GetLayoutMetricsLayoutViewport
        {
            public decimal PageX { get; set; }
            public decimal PageY { get; set; }
        }
    }
}
