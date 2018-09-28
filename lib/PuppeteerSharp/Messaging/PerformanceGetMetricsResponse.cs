using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PerformanceGetMetricsResponse
    {
        [JsonProperty(Constants.METRICS)]
        internal List<Metric> Metrics { get; set; }
    }
}
