namespace PuppeteerSharp.Cdp.Messaging
{
    internal class FetchRequestPausedResponse : RequestWillBeSentResponse
    {
        public ResourceType? ResourceType { get; set; }
    }
}
