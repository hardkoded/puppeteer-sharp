using Newtonsoft.Json;

namespace CefSharp.Puppeteer
{
    internal struct Metric
    {
        public string Name { get; set; }

        public decimal Value { get; set; }
    }
}
