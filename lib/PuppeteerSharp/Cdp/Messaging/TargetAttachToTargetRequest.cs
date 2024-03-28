namespace PuppeteerSharp.Cdp.Messaging
{
    internal class TargetAttachToTargetRequest
    {
        public string TargetId { get; set; }

        public bool Flatten { get; set; }
    }
}
