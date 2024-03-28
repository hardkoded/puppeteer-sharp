namespace PuppeteerSharp.Cdp.Messaging
{
    internal class FetchFailRequest
    {
        public string RequestId { get; set; }

        public string ErrorReason { get; set; }
    }
}
