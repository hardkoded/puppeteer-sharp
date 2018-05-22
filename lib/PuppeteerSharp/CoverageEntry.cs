using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class CoverageEntry
    {
        public string Url { get; set; }
        public CoverageEntryRange[] Ranges { get; set; }
        public string Text { get; internal set; }
    }
}