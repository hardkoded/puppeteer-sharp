namespace PuppeteerSharp.Cdp.Messaging
{
    internal class DomGetBoxModelRequest
    {
        public string ObjectId { get; set; }

        public object BackendNodeId { get; set; }
    }
}
