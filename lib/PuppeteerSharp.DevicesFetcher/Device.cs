using System.Text.Json.Serialization;

namespace PuppeteerSharp.DevicesFetcher
{
    public class Device
    {
        [JsonPropertyName("userAgent")]
        public string UserAgent { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("viewport")]
        public ViewPort Viewport { get; set; }
    }
}
