namespace PuppeteerSharp.Cdp.Messaging
{
    internal class BrowserSetWindowBoundsRequest
    {
        public int WindowId { get; set; }

        public WindowBounds Bounds { get; set; }
    }
}
