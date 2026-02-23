namespace PuppeteerSharp.Cdp.Messaging
{
    internal class StorageSetCookiesRequest
    {
        public string BrowserContextId { get; set; }

        public CookieParam[] Cookies { get; set; }
    }
}
