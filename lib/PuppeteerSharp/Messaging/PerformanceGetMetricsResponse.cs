using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PerformanceGetMetricsResponse
    {
        [JsonProperty("metrics")]
        internal List<Metric> Metrics { get; set; }
    }
}
