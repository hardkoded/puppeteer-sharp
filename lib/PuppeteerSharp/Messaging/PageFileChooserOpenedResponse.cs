namespace PuppeteerSharp.Messaging
{
    internal class PageFileChooserOpenedResponse
    {
        public string Mode { get; set; }

        public string FrameId { get; set; }

        public string BackendNodeId { get; set; }
    }
}
