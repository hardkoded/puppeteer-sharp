using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class GetLayoutMetricsResponse
    {
        [JsonProperty(Constants.LAYOUT_VIEWPORT)]
        public GetLayoutMetricsLayoutViewport LayoutViewport { get; set; }

        internal class GetLayoutMetricsLayoutViewport
        {
            [JsonProperty(Constants.PAGE_X)]
            public decimal PageX { get; set; }

            [JsonProperty(Constants.PAGE_Y)]
            public decimal PageY { get; set; }
        }
    }
}
