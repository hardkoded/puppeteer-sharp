using System.Collections.Generic;

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
}
