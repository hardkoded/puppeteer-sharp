using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    public class PerformanceMetricsResponse
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("metrics")]
        public List<Metric> Metrics { get; set; }
    }
}
