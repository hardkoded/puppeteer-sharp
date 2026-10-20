namespace PuppeteerSharp.Cdp.Messaging
{
    internal class TargetGetTargetInfoResponse
    {
        public TargetInfoWithEmbedderData TargetInfo { get; set; }

        internal class TargetInfoWithEmbedderData
        {
            public TabEmbedderData EmbedderData { get; set; }
        }

        internal class TabEmbedderData
        {
            public int? TabStripIndex { get; set; }

            public bool? TabActive { get; set; }

            public bool? TabPinned { get; set; }

            public string TabGroupId { get; set; }
        }
    }
}
