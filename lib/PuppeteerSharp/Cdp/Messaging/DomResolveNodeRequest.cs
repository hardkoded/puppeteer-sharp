namespace PuppeteerSharp.Cdp.Messaging
{
    internal class DomResolveNodeRequest
    {
        public object BackendNodeId { get; set; }

        public int ExecutionContextId { get; set; }
    }
}
