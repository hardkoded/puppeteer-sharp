namespace PuppeteerSharp
{
    /// <summary>
    /// Console message location.
    /// </summary>
    public class ConsoleMessageLocation
    {
        /// <summary>
        /// URL of the resource if known.
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// Line number in the resource if known.
        /// </summary>
        public int? LineNumber { get; set; }

        /// <summary>
        /// Column number in the resource if known.
        /// </summary>
        public int? ColumnNumber { get; set; }
    }
}