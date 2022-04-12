using CefSharp.Puppeteer.Messaging;

namespace CefSharp.Puppeteer
{
    internal class RedirectInfo
    {
        public RequestWillBeSentPayload Event { get; set; }

        public string FetchRequestId { get; set; }
    }
}
