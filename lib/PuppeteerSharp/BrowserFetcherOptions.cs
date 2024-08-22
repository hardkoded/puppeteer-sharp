using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Browser fetcher options used to construct a <see cref="BrowserFetcher"/>.
    /// </summary>
    public class BrowserFetcherOptions
    {
        /// <summary>
        /// A custom download delegate.
        /// </summary>
        /// <param name="address">address.</param>
        /// <param name="fileName">fileName.</param>
        /// <returns>A Task that resolves when the download finishes.</returns>
        public delegate Task CustomFileDownloadAction(string address, string fileName);

        /// <summary>
        /// Browser. Defaults to Chrome.
        /// </summary>
        public SupportedBrowser Browser { get; set; } = SupportedBrowser.Chrome;

        /// <summary>
        /// Platform. Defaults to current platform.
        /// </summary>
        public Platform? Platform { get; set; }

        /// <summary>
        /// A path for the downloads folder. Defaults to [root]/.local-chromium, where [root] is where the project binaries are located.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// A download host to be used. Defaults to https://storage.googleapis.com.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets the default or a custom download delegate.
        /// </summary>
        public CustomFileDownloadAction CustomFileDownload { get; set; }
    }
}
