using PuppeteerSharp.Media;

namespace PuppeteerSharp.Messaging
{
    internal class PageCaptureScreenshotRequest
    {
        public string Format { get; set; }
        public int Quality { get; set; }
        public Clip Clip { get; set; }
    }
}
