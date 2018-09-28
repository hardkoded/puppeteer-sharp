using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Tests.InputTests
{
    public class Dimensions
    {
        [JsonProperty(Constants.X)]
        public decimal X { get; set; }
        [JsonProperty(Constants.Y)]
        public decimal Y { get; set; }
        [JsonProperty(Constants.WIDTH)]
        public decimal Width { get; set; }
        [JsonProperty(Constants.HEIGHT)]
        public decimal Height { get; set; }
    }
}
