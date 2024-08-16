using System.Text.Json.Serialization;

namespace PuppeteerSharp.DevicesFetcher
{
    public class ViewPort
    {
        [JsonPropertyName("width")]
        public int Width { get; set; }
        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("deviceScaleFactor")]
        public double DeviceScaleFactor { get; set; }

        [JsonPropertyName("isMobile")]
        public bool IsMobile { get; set; }
        [JsonPropertyName("hasTouch")]
        public bool HasTouch { get; set; }
        [JsonPropertyName("isLandscape")]
        public bool IsLandscape { get; set; }
    }
}
