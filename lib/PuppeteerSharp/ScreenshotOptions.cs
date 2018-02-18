using System;

namespace PuppeteerSharp
{
    public class ScreenshotOptions
    {
        public (int, int) Clip { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool FullPage { get; set; }
        public bool OmitBackground { get; set; }
    }
}