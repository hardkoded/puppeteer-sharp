namespace PuppeteerSharp.Cdp.Messaging
{
    internal class FetchRequestPausedResponse : RequestWillBeSentResponse
    {
        public ResourceType? ResourceType { get; set; }

        public string RedirectedRequestId { get; set; }
    }
}
