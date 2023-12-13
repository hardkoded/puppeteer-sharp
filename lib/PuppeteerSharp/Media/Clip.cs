namespace PuppeteerSharp.Media
{
    /// <summary>
    /// Clip data.
    /// </summary>
    /// <seealso cref="ScreenshotOptions.Clip"/>
    public class Clip : BoundingBox
    {
        /// <summary>
        /// Scale of the webpage rendering. Defaults to 1.
        /// </summary>
        /// <value>The scale.</value>
        public int Scale { get; set; } = 1;
    }
}
