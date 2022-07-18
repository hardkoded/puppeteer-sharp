using CefSharp.DevTools.Dom.Messaging;

namespace CefSharp.DevTools.Dom
{
    internal class RedirectInfo
    {
        public RequestWillBeSentPayload Event { get; set; }

        public string FetchRequestId { get; set; }
    }
}
