namespace PuppeteerSharp.Cdp.Messaging
{
    internal class BrowserSetDownloadBehaviorRequest
    {
        public DownloadPolicy Behavior { get; set; }

        public string DownloadPath { get; set; }

        public string BrowserContextId { get; set; }
    }
}
