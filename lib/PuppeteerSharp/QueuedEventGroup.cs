using CefSharp.DevTools.Dom.Messaging;

namespace CefSharp.DevTools.Dom
{
    internal class QueuedEventGroup
    {
        public ResponseReceivedResponse ResponseReceivedEvent { get; set; }

        public LoadingFinishedEventResponse LoadingFinishedEvent { get; set; }

        public LoadingFailedEventResponse LoadingFailedEvent { get; set; }
    }
}
