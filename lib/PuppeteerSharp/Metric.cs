using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal struct Metric
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
    }
}