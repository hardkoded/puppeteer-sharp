namespace CefSharp.DevTools.Dom.Messaging
{
    internal class FetchRequestPausedResponse
    {
        public string RequestId { get; set; }

        public Payload Request { get; set; }

        public string NetworkId { get; set; }
    }
}
