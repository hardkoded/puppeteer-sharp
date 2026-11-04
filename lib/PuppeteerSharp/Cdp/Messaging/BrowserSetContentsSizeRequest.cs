namespace PuppeteerSharp.Cdp.Messaging
{
    internal class BrowserSetContentsSizeRequest
    {
        public int WindowId { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
