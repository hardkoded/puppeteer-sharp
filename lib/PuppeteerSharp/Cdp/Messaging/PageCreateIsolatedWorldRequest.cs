namespace PuppeteerSharp.Cdp.Messaging
{
    internal class PageCreateIsolatedWorldRequest
    {
        public string FrameId { get; set; }

        public string WorldName { get; set; }

        // Kept for backwards compatibility with older Chromium versions (< v130) where the CDP schema had a typo.
        public bool GrantUniveralAccess { get; set; }

        // Added to support modern Chromium versions (v130+) where the typo was corrected.
        public bool GrantUniversalAccess { get; set; }
    }
}
