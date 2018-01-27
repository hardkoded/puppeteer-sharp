namespace PuppeteerSharp
{
    internal class ViewPortOptions
    {
        public ViewPortOptions()
        {
            IsMobile = false;
            DeviceScaleFactor = 1;
        }

        public int Width { get; set; }
        public int Height { get; set; }
        public bool? IsMobile { get; set; }
        public decimal DeviceScaleFactor { get; internal set; }
    }
}