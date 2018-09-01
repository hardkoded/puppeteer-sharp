using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// Launcher controls the creation of Chromium processes or the connection remote ones.
    /// </summary>
    public class Launcher
    {
        #region Private members

        private readonly ILoggerFactory _loggerFactory;
        private ChromiumProcess _chromiumProcess;
        private bool _chromiumLaunched;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the process created by the instance is being closed.
        /// </summary>
        /// <value><c>true</c> if is the process is closed; otherwise, <c>false</c>.</value>
        public bool IsChromeClosing => _chromiumProcess != null || _chromiumProcess.IsClosing;

        /// <summary>
        /// Gets or sets a value indicating whether the process created by the instance is closed.
        /// </summary>
        /// <value><c>true</c> if is the process is closed; otherwise, <c>false</c>.</value>
        public bool IsChromeClosed => _chromiumProcess == null || _chromiumProcess.IsClosed;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Launcher"/> class.
        /// </summary>
        /// <param name="loggerFactory">Logger factory.</param>
        public Launcher(ILoggerFactory loggerFactory = null)
        {
            _loggerFactory = loggerFactory ?? new LoggerFactory();
        }

        #region Public methods
        /// <summary>
        /// The method launches a browser instance with given arguments. The browser will be closed when the Browser is disposed.
        /// </summary>
        /// <param name="options">Options for launching Chrome</param>
        /// <returns>A connected browser.</returns>
        /// <remarks>
        /// See <a href="https://www.howtogeek.com/202825/what%E2%80%99s-the-difference-between-chromium-and-chrome/">this article</a>
        /// for a description of the differences between Chromium and Chrome.
        /// <a href="https://chromium.googlesource.com/chromium/src/+/lkcr/docs/chromium_browser_vs_google_chrome.md">This article</a> describes some differences for Linux users.
        /// </remarks>
        public async Task<Browser> LaunchAsync(LaunchOptions options)
        {
            EnsureSingleLaunchOrConnect();

            var chromiumExecutable = GetOrFetchChromeExecutable(options);
            _chromiumProcess = new ChromiumProcess(chromiumExecutable, options, _loggerFactory);
            try
            {
                await _chromiumProcess.StartAsync().ConfigureAwait(false);
                try
                {
                    var keepAliveInterval = 0;
                    var connection = await Connection
                        .Create(_chromiumProcess.EndPoint, options.SlowMo, keepAliveInterval, _loggerFactory)
                        .ConfigureAwait(false);

                    var browser = await Browser
                        .CreateAsync(connection, Array.Empty<string>(), options.IgnoreHTTPSErrors, !options.AppMode, _chromiumProcess)
                        .ConfigureAwait(false);

                    await EnsureInitialPageAsync(browser).ConfigureAwait(false);
                    return browser;
                }
                catch (Exception ex)
                {
                    throw new ChromiumProcessException("Failed to create connection", ex);
                }
            }
            catch
            {
                await _chromiumProcess.KillAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Attaches Puppeteer to an existing Chromium instance. The browser will be closed when the Browser is disposed.
        /// </summary>
        /// <param name="options">Options for connecting.</param>
        /// <returns>A connected browser.</returns>
        public async Task<Browser> ConnectAsync(ConnectOptions options)
        {
            EnsureSingleLaunchOrConnect();

            try
            {
                var connectionDelay = options.SlowMo;
                var keepAliveInterval = 0;

                var connection = await Connection.Create(options.BrowserWSEndpoint, connectionDelay, keepAliveInterval, _loggerFactory).ConfigureAwait(false);

                var response = await connection.SendAsync<GetBrowserContextsResponse>("Target.getBrowserContexts");

                return await Browser
                    .CreateAsync(connection, response.BrowserContextIds, options.IgnoreHTTPSErrors, true, null)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new ChromiumProcessException("Failed to create connection", ex);
            }
        }

        /// <summary>
        /// Gets the executable path.
        /// </summary>
        /// <returns>The executable path.</returns>
        public static string GetExecutablePath()
            => new BrowserFetcher().RevisionInfo(BrowserFetcher.DefaultRevision).ExecutablePath;

        #endregion

        #region Private methods

        private void EnsureSingleLaunchOrConnect()
        {
            if (_chromiumLaunched)
            {
                throw new InvalidOperationException("Unable to create or connect to another chromium process");
            }

            _chromiumLaunched = true;
        }

        private static string GetOrFetchChromeExecutable(LaunchOptions options)
        {
            var chromeExecutable = options.ExecutablePath;
            if (string.IsNullOrEmpty(chromeExecutable))
            {
                var browserFetcher = new BrowserFetcher();
                chromeExecutable = browserFetcher.RevisionInfo(BrowserFetcher.DefaultRevision).ExecutablePath;
            }

            if (!File.Exists(chromeExecutable))
            {
                throw new FileNotFoundException("Failed to launch chrome! path to executable does not exist", chromeExecutable);
            }

            return chromeExecutable;
        }

        private static Task EnsureInitialPageAsync(Browser browser)
        {
            // Wait for initial page target to be created.
            if (browser.Targets().Any(target => target.Type == TargetType.Page))
            {
                return Task.CompletedTask;
            }
            var initialPageCompletion = new TaskCompletionSource<bool>();
            void InitialPageCallback(object sender, TargetChangedArgs e)
            {
                if (e.Target.Type == TargetType.Page)
                {
                    initialPageCompletion.SetResult(true);
                    browser.TargetCreated -= InitialPageCallback;
                }
            }
            browser.TargetCreated += InitialPageCallback;
            return initialPageCompletion.Task;
        }

        #endregion
    }
}
