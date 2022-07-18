using System.Collections.Generic;

namespace CefSharp.DevTools.Dom.Messaging
{
    internal class PerformanceGetMetricsResponse
    {
        public List<Metric> Metrics { get; set; }
    }
}
