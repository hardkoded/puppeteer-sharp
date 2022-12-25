namespace PuppeteerSharp
{
    /// <summary>
    /// Offset used in conjunction with <see cref="ElementHandle.ClickablePointAsync(Offset?)"/>.
    /// </summary>
    public struct Offset
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Offset"/> struct.
        /// </summary>
        /// <param name="x">x-offset for the clickable point relative to the top-left corner of the border box.</param>
        /// <param name="y">y-offset for the clickable point relative to the top-left corner of the border box.</param>
        public Offset(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// x-offset for the clickable point relative to the top-left corner of the border box.
        /// </summary>
        public decimal X { get; set; }

        /// <summary>
        /// y-offset for the clickable point relative to the top-left corner of the border box.
        /// </summary>
        public decimal Y { get; set; }
    }
}
