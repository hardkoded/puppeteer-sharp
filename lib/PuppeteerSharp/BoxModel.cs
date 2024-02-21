namespace PuppeteerSharp
{
    /// <summary>
    /// Represents boxes of the element.
    /// </summary>
    public class BoxModel
    {
        internal BoxModel()
        {
        }

        /// <summary>
        /// Gets the Content box.
        /// </summary>
        public BoxModelPoint[] Content { get; set; }

        /// <summary>
        /// Gets the Padding box.
        /// </summary>
        public BoxModelPoint[] Padding { get; set; }

        /// <summary>
        /// Gets the Border box.
        /// </summary>
        public BoxModelPoint[] Border { get; set; }

        /// <summary>
        /// Gets the Margin box.
        /// </summary>
        public BoxModelPoint[] Margin { get; set; }

        /// <summary>
        /// Gets the element's width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets the element's height.
        /// </summary>
        public int Height { get; set; }
    }
}
