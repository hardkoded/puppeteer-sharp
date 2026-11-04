namespace PuppeteerSharp
{
    /// <summary>
    /// A media type and its associated file extensions accepted by a <see cref="PWAFileHandler"/>.
    /// </summary>
    public class PWAFileHandlerAccept
    {
        /// <summary>
        /// Gets or sets the mime type, as per
        /// <see href="https://www.iana.org/assignments/media-types/media-types.xhtml">the IANA media types registry</see>.
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// Gets or sets the file extensions associated with <see cref="MediaType"/>.
        /// </summary>
        public string[] FileExtensions { get; set; }
    }
}
