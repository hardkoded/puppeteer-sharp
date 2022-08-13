using System.Collections.Generic;

namespace CefSharp.Dom.Messaging
{
    internal class PerformanceMetricsResponse
    {
        public string Title { get; set; }

        public List<Metric> Metrics { get; set; }
    }
}
