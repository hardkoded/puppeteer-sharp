using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp
{
    internal class RedirectInfo
    {
        public RequestWillBeSentResponse Event { get; set; }

        public string FetchRequestId { get; set; }

        public CDPSession Client { get; set; }

        public CDPSession FetchClient { get; set; }
    }
}
