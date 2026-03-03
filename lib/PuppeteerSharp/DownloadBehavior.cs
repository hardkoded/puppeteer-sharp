namespace PuppeteerSharp
{
    /// <summary>
    /// Behavior definition for when downloading a file.
    /// </summary>
    public class DownloadBehavior
    {
        /// <summary>
        /// Whether to allow all or deny all download requests, or use default behavior if available.
        /// </summary>
        /// <remarks>
        /// Setting this to <see cref="DownloadPolicy.AllowAndName"/> will name all files according to their download GUIDs.
        /// </remarks>
        public DownloadPolicy Policy { get; set; }

        /// <summary>
        /// The default path to save downloaded files to.
        /// </summary>
        /// <remarks>
        /// Setting this is required if <see cref="Policy"/> is set to <see cref="DownloadPolicy.Allow"/> or <see cref="DownloadPolicy.AllowAndName"/>.
        /// </remarks>
        public string DownloadPath { get; set; }
    }
}
