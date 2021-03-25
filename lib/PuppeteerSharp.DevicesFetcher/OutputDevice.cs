namespace PuppeteerSharp.DevicesFetcher
{
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
}
