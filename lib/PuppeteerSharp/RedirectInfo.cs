using CefSharp.Dom.Messaging;

namespace CefSharp.Dom
{
    internal class RedirectInfo
    {
        public RequestWillBeSentPayload Event { get; set; }

        public string FetchRequestId { get; set; }
    }
}
