using CefSharp.Dom.Messaging;

namespace CefSharp.Dom
{
    internal class QueuedEventGroup
    {
        public ResponseReceivedResponse ResponseReceivedEvent { get; set; }

        public LoadingFinishedEventResponse LoadingFinishedEvent { get; set; }

        public LoadingFailedEventResponse LoadingFailedEvent { get; set; }
    }
}
