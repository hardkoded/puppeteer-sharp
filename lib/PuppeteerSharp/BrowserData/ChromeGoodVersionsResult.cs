using System.Collections.Generic;

namespace PuppeteerSharp.BrowserData
{
    internal class ChromeGoodVersionsResult
    {
        public Dictionary<string, ChromeGoodVersionsResultVersion> Channels { get; set; }

        internal class ChromeGoodVersionsResultVersion
        {
            public string Version { get; set; }
        }
    }
}
