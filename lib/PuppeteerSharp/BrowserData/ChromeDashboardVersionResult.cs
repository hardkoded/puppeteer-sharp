using System.Collections.Generic;

namespace PuppeteerSharp.BrowserData
{
    internal class ChromeDashboardVersionResult
    {
        public Dictionary<string, ChromeDashboardDownloadEntry[]> Downloads { get; set; }

        internal class ChromeDashboardDownloadEntry
        {
            public string Platform { get; set; }

            public string Url { get; set; }
        }
    }
}
