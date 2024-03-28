namespace PuppeteerSharp.Cdp.Messaging
{
    internal class LoadingFailedEventResponse
    {
        public string RequestId { get; set; }

        public string ErrorText { get; set; }
    }
}
