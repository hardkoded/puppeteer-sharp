using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PerformanceMetricsResponse
    {
        [JsonProperty("title")]
        internal string Title { get; set; }
        [JsonProperty("metrics")]
        internal List<Metric> Metrics { get; set; }
    }
}
