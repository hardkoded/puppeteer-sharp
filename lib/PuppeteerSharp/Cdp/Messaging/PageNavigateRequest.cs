namespace PuppeteerSharp.Cdp.Messaging
{
    internal class PageNavigateRequest
    {
        public string Url { get; set; }

        public string Referrer { get; set; }

        public string FrameId { get; set; }

        public string ReferrerPolicy { get; set; }
    }
}
