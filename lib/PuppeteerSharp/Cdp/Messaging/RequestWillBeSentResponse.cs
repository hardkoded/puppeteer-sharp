namespace PuppeteerSharp.Cdp.Messaging
{
    internal class RequestWillBeSentResponse
    {
        public string RequestId { get; set; }

        public string LoaderId { get; set; }

        public Request Request { get; set; }

        public ResponsePayload RedirectResponse { get; set; }

        public ResourceType? Type { get; set; }

        public string FrameId { get; set; }

        public bool RedirectHasExtraInfo { get; set; }

        public string NetworkId { get; set; }

        public Initiator Initiator { get; set; }
    }
}
