using CefSharp.DevTools.Dom.Media;

namespace CefSharp.DevTools.Dom.Messaging
{
    internal class PageCaptureScreenshotRequest
    {
        public string Format { get; set; }

        public int Quality { get; set; }

        public Clip Clip { get; set; }
    }
}
