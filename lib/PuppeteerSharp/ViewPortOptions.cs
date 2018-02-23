namespace PuppeteerSharp
{
    public class ViewPortOptions
    {
        public ViewPortOptions()
        {
            IsMobile = false;
            DeviceScaleFactor = 1;
            HasTouch = false;
        }

        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsMobile { get; set; }
        public decimal DeviceScaleFactor { get; internal set; }
        public bool IsLandscape { get; internal set; }
        public bool HasTouch { get; internal set; }
    }
}