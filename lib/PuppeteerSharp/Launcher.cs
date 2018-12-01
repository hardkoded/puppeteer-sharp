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
        private bool _chromiumLaunched;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Launcher"/> class.
        /// </summary>
        /// <param name="loggerFactory">Logger factory.</param>
        public Launcher(ILoggerFactory loggerFactory = null) => _loggerFactory = loggerFactory ?? new LoggerFactory();

        #region Properties
        /// <summary>
        /// Gets Chromium process, if any was created by this launcher.
        /// </summary>
        public ChromiumProcess Process { get; private set; }
        #endregion

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
            Process = new ChromiumProcess(chromiumExecutable, options, _loggerFactory);
            try
            {
                await Process.StartAsync().ConfigureAwait(false);
                try
                {
                    var connection = await Connection
                        .Create(Process.EndPoint, options, _loggerFactory)
                        .ConfigureAwait(false);

                    var browser = await Browser
                        .CreateAsync(connection, Array.Empty<string>(), options.IgnoreHTTPSErrors, options.DefaultViewport, Process)
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
                await Process.KillAsync().ConfigureAwait(false);
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
                var connection = await Connection.Create(options.BrowserWSEndpoint, options, _loggerFactory).ConfigureAwait(false);
                var response = await connection.SendAsync<GetBrowserContextsResponse>("Target.getBrowserContexts");
                return await Browser
                    .CreateAsync(connection, response.BrowserContextIds, options.IgnoreHTTPSErrors, options.DefaultViewport, null)
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
            => ResolveExecutablePath();

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
                chromeExecutable = ResolveExecutablePath();
            }

            if (!File.Exists(chromeExecutable))
            {
                throw new FileNotFoundException("Failed to launch chrome! path to executable does not exist", chromeExecutable);
            }

            return chromeExecutable;
        }

        private static string ResolveExecutablePath()
        {
            var executablePath = Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH");
            if (!string.IsNullOrEmpty(executablePath))
            {
                if (!File.Exists(executablePath))
                {
                    throw new FileNotFoundException("Tried to use PUPPETEER_EXECUTABLE_PATH env variable to launch browser but did not find any executable", executablePath);
                }
                return executablePath;
            }

            var browserFetcher = new BrowserFetcher();
            var revision = Environment.GetEnvironmentVariable("PUPPETEER_CHROMIUM_REVISION");
            RevisionInfo revisionInfo;
            if (!string.IsNullOrEmpty(revision) && int.TryParse(revision, out var revisionNumber))
            {
                revisionInfo = browserFetcher.RevisionInfo(revisionNumber);
                if (!revisionInfo.Local)
                {
                    throw new FileNotFoundException("Tried to use PUPPETEER_CHROMIUM_REVISION env variable to launch browser but did not find executable", revisionInfo.ExecutablePath);
                }
                return revisionInfo.ExecutablePath;
            }
            revisionInfo = browserFetcher.RevisionInfo(BrowserFetcher.DefaultRevision);
            if (!revisionInfo.Local)
            {
                throw new FileNotFoundException("Chromium revision is not downloaded. Run BrowserFetcher.DownloadAsync or download Chromium manually", revisionInfo.ExecutablePath);
            }
            return revisionInfo.ExecutablePath;
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
                    initialPageCompletion.TrySetResult(true);
                    browser.TargetCreated -= InitialPageCallback;
                }
            }
            browser.TargetCreated += InitialPageCallback;
            return initialPageCompletion.Task;
        }

        #endregion
    }
}