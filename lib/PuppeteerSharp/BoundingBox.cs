namespace PuppeteerSharp
{
    /// <summary>
    /// Bounding box data returned by <see cref="ElementHandle.BoundingBoxAsync"/>.
    /// </summary>
    [System.Obsolete("Use PuppeteerSharp.Abstractions.BoundingBox class instead")]
    public class BoundingBox : Abstractions.BoundingBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> class.
        /// </summary>
        public BoundingBox() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public BoundingBox(decimal x, decimal y, decimal width, decimal height)
            : base(x, y, width, height)
        {
        }
    }
}