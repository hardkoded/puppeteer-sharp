namespace PuppeteerSharp.Cdp.Messaging
{
    internal class TargetCreateTargetRequest
    {
        public string Url { get; set; }

        public object BrowserContextId { get; set; }

        public bool Background { get; set; }
    }
}
