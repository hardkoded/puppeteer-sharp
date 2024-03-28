namespace PuppeteerSharp.Cdp.Messaging
{
    internal class PageGetFrameTree
    {
        public FramePayload Frame { get; set; }

        public PageGetFrameTree[] ChildFrames { get; set; }
    }
}
