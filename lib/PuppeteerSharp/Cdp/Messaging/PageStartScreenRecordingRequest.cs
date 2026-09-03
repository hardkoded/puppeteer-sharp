namespace PuppeteerSharp.Cdp.Messaging
{
    internal class PageStartScreenRecordingRequest
    {
        public bool? Audio { get; set; }

        public int? MaxWidth { get; set; }

        public int? MaxHeight { get; set; }

        public int? FrameRate { get; set; }
    }
}
