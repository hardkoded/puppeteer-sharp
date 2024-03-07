namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a point.
    /// </summary>
    public record BoxModelPoint
    {
        /// <summary>
        /// The X coordinate.
        /// </summary>
        public decimal X { get; set; }

        /// <summary>
        /// The y coordinate.
        /// </summary>
        public decimal Y { get; set; }
    }
}
