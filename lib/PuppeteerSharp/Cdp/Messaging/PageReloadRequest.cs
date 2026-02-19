namespace PuppeteerSharp.Cdp.Messaging
{
    internal class PageReloadRequest
    {
        public string FrameId { get; set; }

        public bool IgnoreCache { get; set; }
    }
}
