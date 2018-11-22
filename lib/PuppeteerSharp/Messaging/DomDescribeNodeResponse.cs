namespace PuppeteerSharp.Messaging
{
    internal class DomDescribeNodeResponse
    {
        public DomNode Node { get; set; }

        internal class DomNode
        {
            public string FrameId { get; set; }
        }
    }
}