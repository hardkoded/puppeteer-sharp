namespace PuppeteerSharp.Cdp.Messaging
{
    internal class LoadingFinishedEventResponse
    {
        public string RequestId { get; set; }

        public long EncodedDataLength { get; set; }
    }
}
