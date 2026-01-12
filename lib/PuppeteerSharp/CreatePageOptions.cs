namespace PuppeteerSharp
{
    /// <summary>
    /// Options for creating a new page.
    /// </summary>
    public class CreatePageOptions
    {
        /// <summary>
        /// The type of page to create. Defaults to "tab".
        /// </summary>
        public string Type { get; set; } = "tab";

        /// <summary>
        /// Window bounds for creating a new window. Only applies when Type is "window".
        /// </summary>
        public WindowBounds WindowBounds { get; set; }

        /// <summary>
        /// Whether to create the page in the background.
        /// </summary>
        /// <remarks>
        /// Defaults to false.
        /// </remarks>
        public bool Background { get; set; }
    }
}
