using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    public class PerformanceGetMetricsResponse
    {
        [JsonProperty("metrics")]
        public List<Metric> Metrics { get; set; }
    }
}
