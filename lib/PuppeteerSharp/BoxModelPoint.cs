namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a <see cref="BoxModel"/> point
    /// </summary>
    [System.Obsolete("Use PuppeteerSharp.Abstractions.BoxModelPoint class instead")]
    public struct BoxModelPoint
    {
        /// <summary>
        /// Gets the X point
        /// </summary>
        public decimal X { get; set; }

        /// <summary>
        /// Gets the y point
        /// </summary>
        public decimal Y { get; set; }
    }
}