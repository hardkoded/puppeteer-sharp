using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Cdp
{
    /// <summary>
    /// A stored <c>Network.requestWillBeSent</c> event. With request interception enabled, it can also
    /// hold a <c>Network.loadingFailed</c> event that arrived before the <c>Fetch.requestPaused</c> event
    /// that creates the request.
    /// </summary>
    internal class StoredRequestWillBeSent
    {
        public RequestWillBeSentResponse Event { get; set; }

        public CDPSession Client { get; set; }

        public StoredLoadingFailed LoadingFailed { get; set; }
    }
}
