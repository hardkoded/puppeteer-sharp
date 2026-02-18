namespace PuppeteerSharp.Cdp.Messaging
{
    internal class BrowserSetPermissionRequest
    {
        public string Origin { get; set; }

        public string BrowserContextId { get; set; }

        public BrowserPermissionDescriptor Permission { get; set; }

        public PermissionState Setting { get; set; }
    }
}
