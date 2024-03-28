namespace PuppeteerSharp.Cdp.Messaging
{
    internal class ConnectionResponseParams
    {
        public string SessionId { get; set; }

        public string Message { get; set; }

        public string Stream { get; set; }

        public TargetInfo TargetInfo { get; set; }
    }
}
