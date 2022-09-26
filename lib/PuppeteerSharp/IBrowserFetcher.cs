using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// BrowserFetcher can download and manage different versions of Chromium.
    /// BrowserFetcher operates on revision strings that specify a precise version of Chromium, e.g. 533271. Revision strings can be obtained from omahaproxy.appspot.com.
    /// </summary>
    /// <example>
    /// Example on how to use BrowserFetcher to download a specific version of Chromium and run Puppeteer against it:
    /// <code>
    /// var browserFetcher = Puppeteer.CreateBrowserFetcher();
    /// var revisionInfo = await browserFetcher.DownloadAsync("533271");
    /// var browser = await await Puppeteer.LaunchAsync(new LaunchOptions { ExecutablePath = revisionInfo.ExecutablePath});
    /// </code>
    /// </example>
    public interface IBrowserFetcher : IDisposable
    {
        /// <summary>
        /// Occurs when download progress in <see cref="DownloadAsync()"/>/<see cref="DownloadAsync(string)"/> changes.
        /// </summary>
        event DownloadProgressChangedEventHandler DownloadProgressChanged;

        /// <summary>
        /// Default Firefox revision.
        /// </summary>
        string DefaultFirefoxRevision { get; }

        /// <summary>
        /// A download host to be used. Defaults to https://storage.googleapis.com.
        /// </summary>
        string DownloadHost { get; }

        /// <summary>
        /// Gets the downloads folder.
        /// </summary>
        string DownloadsFolder { get; }

        /// <summary>
        /// Gets the platform.
        /// </summary>
        Platform Platform { get; }

        /// <summary>
        /// Gets the product.
        /// </summary>
        Product Product { get; }

        /// <summary>
        /// Proxy used by the WebClient in <see cref="DownloadAsync()"/>, <see cref="DownloadAsync(string)"/> and <see cref="CanDownloadAsync"/>
        /// </summary>
        IWebProxy WebProxy { get; set; }

        /// <summary>
        /// The method initiates a HEAD request to check if the revision is available.
        /// </summary>
        /// <returns>Whether the version is available or not.</returns>
        /// <param name="revision">A revision to check availability.</param>
        Task<bool> CanDownloadAsync(string revision);

        /// <summary>
        /// Downloads the revision.
        /// </summary>
        /// <returns>Task which resolves to the completed download.</returns>
        Task<RevisionInfo> DownloadAsync();

        /// <summary>
        /// Downloads the revision.
        /// </summary>
        /// <returns>Task which resolves to the completed download.</returns>
        /// <param name="revision">Revision.</param>
        Task<RevisionInfo> DownloadAsync(string revision);

        /// <summary>
        /// Gets the executable path for a revision.
        /// </summary>
        /// <returns>The executable path.</returns>
        /// <param name="revision">Revision.</param>
        string GetExecutablePath(string revision);

        /// <summary>
        /// Gets the revision info.
        /// </summary>
        /// <returns>Revision info.</returns>
        Task<RevisionInfo> GetRevisionInfoAsync();

        /// <summary>
        /// A list of all revisions available locally on disk.
        /// </summary>
        /// <returns>The available revisions.</returns>
        IEnumerable<string> LocalRevisions();

        /// <summary>
        /// Removes a downloaded revision.
        /// </summary>
        /// <param name="revision">Revision to remove.</param>
        void Remove(string revision);

        /// <summary>
        /// Gets the revision info.
        /// </summary>
        /// <returns>Revision info.</returns>
        /// <param name="revision">A revision to get info for.</param>
        RevisionInfo RevisionInfo(string revision);
    }
}
