namespace PuppeteerSharp.Cdp.Messaging
{
    internal class PageCreateIsolatedWorldRequest
    {
        public string FrameId { get; set; }

        public string WorldName { get; set; }

        public bool GrantUniveralAccess { get; set; }
    }
}
