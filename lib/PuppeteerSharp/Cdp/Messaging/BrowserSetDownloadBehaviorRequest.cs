namespace PuppeteerSharp.Cdp.Messaging
{
    internal class BrowserSetDownloadBehaviorRequest
    {
        public string Behavior { get; set; }

        public string DownloadPath { get; set; }

        public string BrowserContextId { get; set; }

        public bool? EventsEnabled { get; set; }
    }
}
