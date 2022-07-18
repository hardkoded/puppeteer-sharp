namespace CefSharp.DevTools.Dom.Messaging
{
    internal class PageFileChooserOpenedResponse
    {
        public string Mode { get; set; }

        public string FrameId { get; set; }

        public object BackendNodeId { get; set; }
    }
}
