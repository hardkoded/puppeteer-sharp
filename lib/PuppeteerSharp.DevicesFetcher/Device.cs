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
}
