using Newtonsoft.Json;

namespace PuppeteerSharp.DevicesFetcher
{
    public class RootObject
    {
        public Extension[] Extensions { get; set; }

        public class Extension
        {
            public string Type { get; set; }
            public Device Device { get; set; }
        }

        public class Device
        {
            [JsonProperty("show-by-default")]
            public bool ShowByDefault { get; set; }
            public string Title { get; set; }
            public Screen Screen { get; set; }
            public string[] Capabilities { get; set; }
            [JsonProperty("user-agent")]
            public string UserAgent { get; set; }
            public string Type { get; set; }
        }

        public class Screen
        {
            public ViewportPayload Horizontal { get; set; }
            [JsonProperty("device-pixel-ratio")]
            public double DevicePixelRatio { get; set; }
            public ViewportPayload Vertical { get; set; }
        }
    }
}
