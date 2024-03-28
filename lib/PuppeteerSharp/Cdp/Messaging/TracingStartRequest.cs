namespace PuppeteerSharp.Cdp.Messaging
{
    internal class TracingStartRequest
    {
        public string Categories { get; set; }

        public string TransferMode { get; set; }
    }
}
