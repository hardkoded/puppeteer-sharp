using Newtonsoft.Json;

namespace PuppeteerSharp.DevicesFetcher
{

    public class Device
    {
        [JsonProperty("userAgent")]
        public string UserAgent { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("viewport")]
        public ViewPort Viewport { get; set; }
    }

    public class ViewPort
    {
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("deviceScaleFactor")]
        public double DeviceScaleFactor { get; set; }

        [JsonProperty("isMobile")]
        public bool IsMobile { get; set; }
        [JsonProperty("hasTouch")]
        public bool HasTouch { get; set; }
        [JsonProperty("isLandscape")]
        public bool IsLandscape { get; set; }
    }
}
