using System.Collections.Generic;

namespace PuppeteerSharp.PageCoverage
{
    /// <summary>
    /// Coverage report for all non-anonymous scripts.
    /// </summary>
    public class CoverageEntry
    {
        /// <summary>
        /// Script URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Script ranges that were executed. Ranges are sorted and non-overlapping.
        /// </summary>
        public CoverageEntryRange[] Ranges { get; set; }

        /// <summary>
        /// Script content.
        /// </summary>
        public string Text { get; set; }
    }
}
