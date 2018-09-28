using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PerformanceMetricsResponse
    {
        [JsonProperty(Constants.TITLE)]
        internal string Title { get; set; }
        [JsonProperty(Constants.METRICS)]
        internal List<Metric> Metrics { get; set; }
    }
}
