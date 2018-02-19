using System;

namespace PuppeteerSharp
{
    public class ScreenshotOptions
    {
        public ScreenshotOptions()
        {
        }

        public Clip Clip { get; set; }
        public bool FullPage { get; set; }
        public bool OmitBackground { get; set; }
        public string Type { get; set; }
        public decimal? Quality { get; set; }
    }
}