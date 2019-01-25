namespace PuppeteerSharp.Messaging
{
    internal class PageNavigateRequest
    {
        public string Url { get; set; }
        public string Referrer { get; set; }
        public string FrameId { get; set; }
    }
}
