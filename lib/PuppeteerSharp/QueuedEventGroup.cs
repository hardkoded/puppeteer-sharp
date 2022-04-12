using CefSharp.Puppeteer.Messaging;

namespace CefSharp.Puppeteer
{
    internal class QueuedEventGroup
    {
        public ResponseReceivedResponse ResponseReceivedEvent { get; set; }

        public LoadingFinishedEventResponse LoadingFinishedEvent { get; set; }

        public LoadingFailedEventResponse LoadingFailedEvent { get; set; }
    }
}
