namespace PuppeteerSharp
{
    /// <summary>
    /// Options for creating a new page.
    /// </summary>
    public class CreatePageOptions
    {
        /// <summary>
        /// Gets or sets the type of page to create.
        /// Use <c>"tab"</c> (default) or <c>"window"</c>.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the window bounds when <see cref="Type"/> is <c>"window"</c>.
        /// </summary>
        public WindowBounds WindowBounds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to create the page in the background.
        /// </summary>
        /// <value><c>false</c> by default.</value>
        public bool? Background { get; set; }
    }
}
