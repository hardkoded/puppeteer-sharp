namespace PuppeteerSharp
{
    /// <summary>
    /// Options for creating a new page.
    /// </summary>
    public class CreatePageOptions
    {
        /// <summary>
        /// The type of page to create. Defaults to <see cref="CreatePageType.Tab"/>.
        /// </summary>
        public CreatePageType Type { get; set; } = CreatePageType.Tab;

        /// <summary>
        /// Window bounds for creating a new window. Only applies when Type is <see cref="CreatePageType.Window"/>.
        /// </summary>
        public WindowBounds WindowBounds { get; set; }

        /// <summary>
        /// Whether to create the page in the background.
        /// When not set or null, the page is created in the foreground.
        /// </summary>
        public bool? Background { get; set; }
    }
}
