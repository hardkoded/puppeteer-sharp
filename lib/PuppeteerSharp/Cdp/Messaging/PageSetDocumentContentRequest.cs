namespace PuppeteerSharp.Cdp.Messaging
{
    internal class PageSetDocumentContentRequest
    {
        public string FrameId { get; set; }

        public string Html { get; set; }
    }
}
