namespace PuppeteerSharp.Cdp.Messaging
{
    internal class TracingStartRequest
    {
        public string TransferMode { get; set; }

        public TracingTraceConfig TraceConfig { get; set; }
    }
}
