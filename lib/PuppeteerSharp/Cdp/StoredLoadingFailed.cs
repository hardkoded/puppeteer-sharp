using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Cdp
{
    internal class StoredLoadingFailed
    {
        public LoadingFailedEventResponse Event { get; set; }

        public CDPSession Client { get; set; }
    }
}
