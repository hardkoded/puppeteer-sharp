namespace PuppeteerSharp.PageCoverage
{
    /// <summary>
    /// Script range.
    /// </summary>
    public class CoverageEntryRange
    {
        /// <summary>
        /// A start offset in text, inclusive.
        /// </summary>
        /// <value>Start offset.</value>
        public int Start { get; internal set; }
        /// <summary>
        /// An end offset in text, exclusive.
        /// </summary>
        /// <value>End offset.</value>
        public int End { get; internal set; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null && GetType() != obj.GetType())
            {
                return false;
            }

            var range = obj as CoverageEntryRange;

            return range.Start == Start &&
               range.End == End;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => Start.GetHashCode() * 397 ^ End.GetHashCode() * 397;
    }
}