namespace PuppeteerSharp
{
    /// <summary>
    /// Represents the download behavior configuration.
    /// </summary>
    public class DownloadBehavior
    {
        /// <summary>
        /// Whether to allow all or deny all download requests, or use default behavior if available.
        /// </summary>
        /// <remarks>
        /// Setting this to <see cref="DownloadPolicy.AllowAndName"/> will name all files according to their download guids.
        /// </remarks>
        public DownloadPolicy Policy { get; set; }

        /// <summary>
        /// The default path to save downloaded files to.
        /// </summary>
        /// <remarks>
        /// Setting this is required if behavior is set to <see cref="DownloadPolicy.Allow"/> or <see cref="DownloadPolicy.AllowAndName"/>.
        /// </remarks>
        public string DownloadPath { get; set; }
    }
}
