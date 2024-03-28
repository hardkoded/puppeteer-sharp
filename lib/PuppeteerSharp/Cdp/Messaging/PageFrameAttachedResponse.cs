namespace PuppeteerSharp.Cdp.Messaging
{
    internal class PageFrameAttachedResponse
    {
        public string FrameId { get; set; }

        public string ParentFrameId { get; set; }
    }
}
