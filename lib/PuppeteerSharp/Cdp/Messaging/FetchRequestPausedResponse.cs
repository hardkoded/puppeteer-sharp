namespace PuppeteerSharp.Cdp.Messaging
{
    internal class FetchRequestPausedResponse : RequestWillBeSentPayload
    {
        public ResourceType? ResourceType { get; set; }
    }
}
