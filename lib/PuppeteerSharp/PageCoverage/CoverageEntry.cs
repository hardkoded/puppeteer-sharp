using System.Collections.Generic;

namespace PuppeteerSharp.PageCoverage
{
    /// <summary>
    /// Coverage report for all non-anonymous scripts.
    /// </summary>
    public class CoverageEntry
    {
        /// <summary>
        /// Script URL
        /// </summary>
        /// <value>Script URL.</value>
        public string Url { get; set; }
        /// <summary>
        /// Script ranges that were executed. Ranges are sorted and non-overlapping.
        /// </summary>
        /// <value>Ranges.</value>
        public CoverageEntryRange[] Ranges { get; set; }
        /// <summary>
        /// Script content
        /// </summary>
        /// <value>Script content.</value>
        public string Text { get; set; }
    }
}