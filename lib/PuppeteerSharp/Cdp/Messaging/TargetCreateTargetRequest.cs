namespace PuppeteerSharp.Cdp.Messaging
{
    internal class TargetCreateTargetRequest
    {
        public string Url { get; set; }

        public object BrowserContextId { get; set; }

        public int? Left { get; set; }

        public int? Top { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public WindowState? WindowState { get; set; }

        public bool? NewWindow { get; set; }

        public bool? Background { get; set; }
    }
}
