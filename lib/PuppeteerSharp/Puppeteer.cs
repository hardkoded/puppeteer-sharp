using Microsoft.Extensions.Logging;
using PuppeteerSharp.Mobile;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        internal const int DefaultTimeout = 30_000;

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
        /// Returns an array of argument based on the options provided and the platform where the library is running 
        /// </summary>
        /// <returns>Chromium arguments.</returns>
        /// <param name="options">Options.</param>
        public static string[] GetDefaultArgs(LaunchOptions options = null)
            => ChromiumProcess.GetDefaultArgs(options ?? new LaunchOptions());

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
        /// 
        /// Environment Variables
        /// Puppeteer looks for certain <see href="https://en.wikipedia.org/wiki/Environment_variable">environment variables</see>() to aid its operations.
        /// - <c>PUPPETEER_CHROMIUM_REVISION</c> - specify a certain version of Chromium you'd like Puppeteer to use. See <see cref="Puppeteer.LaunchAsync(LaunchOptions, ILoggerFactory)"/> on how executable path is inferred. 
        ///   **BEWARE**: Puppeteer is only <see href="https://github.com/GoogleChrome/puppeteer/#q-why-doesnt-puppeteer-vxxx-work-with-chromium-vyyy">guaranteed to work</see> with the bundled Chromium, use at your own risk.
        /// - <c>PUPPETEER_EXECUTABLE_PATH</c> - specify an executable path to be used in <see cref="Puppeteer.LaunchAsync(LaunchOptions, ILoggerFactory)"/>. 
        ///   **BEWARE**: Puppeteer is only <see href="https://github.com/GoogleChrome/puppeteer/#q-why-doesnt-puppeteer-vxxx-work-with-chromium-vyyy">guaranteed to work</see> with the bundled Chromium, use at your own risk.
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

        /// <summary>
        /// Returns a list of devices to be used with <seealso cref="Page.EmulateAsync(DeviceDescriptor)"/>. 
        /// </summary>
        /// <example>
        /// <code>
        ///<![CDATA[
        /// var iPhone = Puppeteer.Devices[DeviceDescriptorName.IPhone6];
        /// using(var page = await browser.NewPageAsync())
        /// {
        ///     await page.EmulateAsync(iPhone);
        ///     await page.goto('https://www.google.com');
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static IReadOnlyDictionary<DeviceDescriptorName, DeviceDescriptor> Devices => DeviceDescriptors.ToReadOnly();
    }
}