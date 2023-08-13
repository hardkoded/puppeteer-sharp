using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using PuppeteerSharp.BrowserData;

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
        /// A download host to be used.
        /// </summary>
        string BaseUrl { get; set; }

        /// <summary>
        /// Determines the path to download browsers to.
        /// </summary>
        string CacheDir { get; set; }

        /// <summary>
        /// Gets the platform.
        /// </summary>
        Platform Platform { get; set; }

        /// <summary>
        /// Gets the browser.
        /// </summary>
        SupportedBrowser Browser { get; set; }

        /// <summary>
        /// Proxy used by the WebClient in <see cref="DownloadAsync()"/>, <see cref="DownloadAsync(string)"/> and <see cref="CanDownloadAsync"/>.
        /// </summary>
        IWebProxy WebProxy { get; set; }

        /// <summary>
        /// The method initiates a HEAD request to check if the revision is available.
        /// </summary>
        /// <returns>Whether the version is available or not.</returns>
        /// <param name="buildId">A build to check availability.</param>
        Task<bool> CanDownloadAsync(string buildId);

        /// <summary>
        /// Downloads the revision.
        /// </summary>
        /// <returns>Task which resolves to the completed download.</returns>
        Task<InstalledBrowser> DownloadAsync();

        /// <summary>
        /// Downloads the revision.
        /// </summary>
        /// <param name="tag">Browser tag.</param>
        /// <returns>Task which resolves to the completed download.</returns>
        Task<InstalledBrowser> DownloadAsync(BrowserTag tag);

        /// <summary>
        /// Downloads the revision.
        /// </summary>
        /// <returns>Task which resolves to the completed download.</returns>
        /// <param name="revision">Revision.</param>
        Task<InstalledBrowser> DownloadAsync(string revision);

        /// <summary>
        /// A list of all browsers available locally on disk.
        /// </summary>
        /// <returns>The available browsers.</returns>
        IEnumerable<InstalledBrowser> GetInstalledBrowsers();

        /// <summary>
        /// Removes a downloaded browser.
        /// </summary>
        /// <param name="buildId">Browser to remove.</param>
        void Uninstall(string buildId);
    }
}
