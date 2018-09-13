using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp.DevicesFetcher
{
    public class DevicePayload
    {
        public string Type { get; set; }

        public string UserAgent { get; set; }

        public ViewportPayload Vertical { get; set; }

        public ViewportPayload Horizontal { get; set; }

        public double DeviceScaleFactor { get; set; }

        public HashSet<string> Capabilities { get; set; }
    }

    public class ViewportPayload
    {
        [JsonProperty("width")]
        public double Width { get; set; }

        [JsonProperty("height")]
        public double Height { get; set; }
    }

    public class OutputDevice
    {
        public string Name { get; set; }
        public string UserAgent { get; set; }
        public OutputDeviceViewport Viewport { get; set; }

        public class OutputDeviceViewport
        {
            public double Width { get; set; }

            public double Height { get; set; }

            public double DeviceScaleFactor { get; set; }

            public bool IsMobile { get; set; }

            public bool HasTouch { get; set; }

            public bool IsLandscape { get; set; }
        }
    }

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
