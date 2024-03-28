using System.Collections.Generic;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class PerformanceGetMetricsResponse
    {
        public List<Metric> Metrics { get; set; }
    }
}
