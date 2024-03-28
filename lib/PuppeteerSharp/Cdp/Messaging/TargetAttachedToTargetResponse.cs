namespace PuppeteerSharp.Cdp.Messaging
{
    internal class TargetAttachedToTargetResponse
    {
        public TargetInfo TargetInfo { get; set; }

        public string SessionId { get; set; }
    }
}
