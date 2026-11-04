namespace PuppeteerSharp
{
    /// <summary>
    /// A file handler registered by a Progressive Web App (PWA) with the OS.
    /// </summary>
    public class PWAFileHandler
    {
        /// <summary>
        /// Gets or sets the URL that opens when the OS invokes this file handler.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the media types and file extensions accepted by this file handler.
        /// </summary>
        public PWAFileHandlerAccept[] Accepts { get; set; }

        /// <summary>
        /// Gets or sets the display name of the file handler.
        /// </summary>
        public string DisplayName { get; set; }
    }
}
