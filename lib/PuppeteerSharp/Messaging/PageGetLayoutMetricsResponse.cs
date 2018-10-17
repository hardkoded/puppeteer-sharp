using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PageGetLayoutMetricsResponse
    {
        [JsonProperty("contentSize")]
        public LayourContentSize ContentSize { get; set; }

        public class LayourContentSize
        {
            [JsonProperty("width")]
            public decimal Width { get; set; }
            [JsonProperty("height")]
            public decimal Height { get; set; }
        }
    }
}
