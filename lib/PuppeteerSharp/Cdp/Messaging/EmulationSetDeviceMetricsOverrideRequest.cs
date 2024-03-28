using PuppeteerSharp.Media;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class EmulationSetDeviceMetricsOverrideRequest
    {
        public bool Mobile { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public double DeviceScaleFactor { get; set; }

        public ScreenOrientation ScreenOrientation { get; set; }
    }
}
