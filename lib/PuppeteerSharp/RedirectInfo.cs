using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp
{
    internal class RedirectInfo
    {
        public RequestWillBeSentPayload Event { get; set; }

        public string FetchRequestId { get; set; }
    }
}
