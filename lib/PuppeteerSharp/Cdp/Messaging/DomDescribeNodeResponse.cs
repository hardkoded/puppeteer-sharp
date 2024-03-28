namespace PuppeteerSharp.Cdp.Messaging
{
    internal class DomDescribeNodeResponse
    {
        public DomNode Node { get; set; }

        internal class DomNode
        {
            public string FrameId { get; set; }

            public object BackendNodeId { get; set; }
        }
    }
}
