using Newtonsoft.Json;

namespace PuppeteerSharp.DevicesFetcher
{
    public class RootObject
    {
        [JsonProperty("extensions")]
        public Extension[] Extensions { get; set; }

        public class Extension
        {
            [JsonProperty("type")]
            public string Type { get; set; }
            [JsonProperty("device")]
            public Device Device { get; set; }
        }

        public class Device
        {
            [JsonProperty("show-by-default")]
            public bool ShowByDefault { get; set; }
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("screen")]
            public Screen Screen { get; set; }
            [JsonProperty("capabilities")]
            public string[] Capabilities { get; set; }
            [JsonProperty("user-agent")]
            public string UserAgent { get; set; }
            [JsonProperty("type")]
            public string Type { get; set; }            
        }

        public class Screen
        {
            [JsonProperty("horizontal")]
            public ViewportPayload Horizontal { get; set; }
            [JsonProperty("device-pixel-ratio")]
            public double DevicePixelRatio { get; set; }
            [JsonProperty("vertical")]
            public ViewportPayload Vertical { get; set; }
        }
    }
}
