using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class GetLayoutMetricsResponse
    {
        [JsonProperty("layoutViewport")]
        public GetLayoutMetricsLayoutViewport LayoutViewport { get; set; }

        internal class GetLayoutMetricsLayoutViewport
        {
            [JsonProperty("pageX")]
            public decimal PageX { get; set; }

            [JsonProperty("pageY")]
            public decimal PageY { get; set; }
        }
    }
}
