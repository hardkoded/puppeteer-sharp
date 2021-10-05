namespace CefSharp.Puppeteer.Messaging
{
    internal class PageFrameDetachedResponse
    {
        public string FrameId { get; set; }

        public string Reason { get; set; }
    }
}
