using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Provides a method to launch a Chromium instance.
    /// </summary>
    /// <example>
    /// The following is a typical example of using a Puppeteer to drive automation:
    /// <code>
    /// var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
    /// var page = await browser.NewPageAsync();
    /// await page.GoToAsync("https://www.google.com");
    /// await Browser.CloseAsync();
    /// </code>
    /// </example>
    public static class Puppeteer
    {
        /// <summary>
        /// The default flags that Chromium will be launched with.
        /// </summary>
        public static string[] DefaultArgs => ChromiumProcess.DefaultArgs;

        /// <summary>
        /// A path where Puppeteer expects to find bundled Chromium. Chromium might not exist there if the downloader was not used.
        /// </summary>
        /// <returns>The path to chrome.exe</returns>
        public static string GetExecutablePath() => Launcher.GetExecutablePath();

        /// <summary>
        /// The method launches a browser instance with given arguments. The browser will be closed when the Browser is disposed.
        /// </summary>
        /// <param name="options">Options for launching Chrome</param>
        /// <param name="loggerFactory">The logger factory</param>
        /// <returns>A connected browser.</returns>
        /// <remarks>
        /// See <a href="https://www.howtogeek.com/202825/what%E2%80%99s-the-difference-between-chromium-and-chrome/">this article</a>
        /// for a description of the differences between Chromium and Chrome.
        /// <a href="https://chromium.googlesource.com/chromium/src/+/lkcr/docs/chromium_browser_vs_google_chrome.md">This article</a> describes some differences for Linux users.
        /// </remarks>
        public static Task<Browser> LaunchAsync(LaunchOptions options, ILoggerFactory loggerFactory = null)
            => new Launcher(loggerFactory).LaunchAsync(options);

        /// <summary>
        /// Attaches Puppeteer to an existing Chromium instance. The browser will be closed when the Browser is disposed.
        /// </summary>
        /// <param name="options">Options for connecting.</param>
        /// <param name="loggerFactory">The logger factory</param>
        /// <returns>A connected browser.</returns>
        public static Task<Browser> ConnectAsync(ConnectOptions options, ILoggerFactory loggerFactory = null)
            => new Launcher(loggerFactory).ConnectAsync(options);

        /// <summary>
        /// Creates the browser fetcher.
        /// </summary>
        /// <returns>The browser fetcher.</returns>
        /// <param name="options">Options.</param>
        public static BrowserFetcher CreateBrowserFetcher(BrowserFetcherOptions options)
            => new BrowserFetcher(options);
    }
}