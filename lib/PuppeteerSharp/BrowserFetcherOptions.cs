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
        /// <remarks>
        /// The default download uses <see cref="System.Net.Http.HttpClient"/>, which trusts the certificates of the operating system.
        /// If browser downloads must trust custom certificates (for example behind a corporate proxy), install the certificate
        /// in the system store (on Linux you can also point the SSL_CERT_FILE or SSL_CERT_DIR environment variables to it),
        /// or provide a delegate here that downloads the file with an <see cref="System.Net.Http.HttpClient"/> configured with your certificates.
        /// </remarks>
        public CustomFileDownloadAction CustomFileDownload { get; set; }
    }
}
