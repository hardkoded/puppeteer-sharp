namespace PuppeteerSharp.PageCoverage
{
    /// <summary>
    /// Coverage data for a source range.
    /// </summary>
    public record CoverageRange
    {
        /// <summary>
        /// JavaScript script source offset for the range start.
        /// </summary>
        public int StartOffset { get; set; }

        /// <summary>
        /// JavaScript script source offset for the range end.
        /// </summary>
        public int EndOffset { get; set; }

        /// <summary>
        /// Collected execution count of the source range.
        /// </summary>
        public int Count { get; set; }
    }
}
