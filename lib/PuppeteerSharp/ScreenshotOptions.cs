using System;
using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public class ScreenshotOptions
    {
        public ScreenshotOptions()
        {
        }

        [JsonProperty("clip")]
        public Clip Clip { get; set; }
        [JsonProperty("fullPage")]
        public bool FullPage { get; set; }
        [JsonProperty("omitBackground")]
        public bool OmitBackground { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("quality")]
        public decimal? Quality { get; set; }
    }
}