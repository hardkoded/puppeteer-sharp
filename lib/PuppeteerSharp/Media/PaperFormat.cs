namespace PuppeteerSharp.Media
{
    /// <summary>
    /// Paper format.
    /// </summary>
    /// <seealso cref="PdfOptions.Format"/>
    [System.Obsolete("Use PuppeteerSharp.Abstractions.Media.PaperFormat class instead")]
    public class PaperFormat : Abstractions.Media.PaperFormat
    {
        /// <summary>
        /// Page width and height in inches.
        /// </summary>
        /// <param name="width">Page width in inches</param>
        /// <param name="height">Page height in inches</param>
        public PaperFormat(decimal width, decimal height)
            : base(width, height)
        {
        }
    }
}