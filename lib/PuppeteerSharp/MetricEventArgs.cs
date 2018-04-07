using System;
using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class MetricEventArgs : EventArgs
    {
        public string Title { get; }
        public Dictionary<string, decimal> Metrics { get; }

        public MetricEventArgs(string title, Dictionary<string, decimal> metrics)
        {
            Title = title;
            Metrics = metrics;
        }
    }
}