using System.Collections.Generic;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class PerformanceMetricsResponse
    {
        public string Title { get; set; }

        public List<Metric> Metrics { get; set; }
    }
}
