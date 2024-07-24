using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.BrowserData;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// Launcher controls the creation of processes or the connection remote ones.
    /// </summary>
    public class Launcher
    {
        private readonly ILoggerFactory _loggerFactory;
        private bool _processLaunched;
        private SupportedBrowser _browser;

        /// <summary>
        /// Initializes a new instance of the <see cref="Launcher"/> class.
        /// </summary>
        /// <param name="loggerFactory">Logger factory.</param>
        public Launcher(ILoggerFactory loggerFactory = null) => _loggerFactory = loggerFactory ?? new LoggerFactory();

        /// <summary>
        /// Gets the process, if any was created by this launcher.
        /// </summary>
        public LauncherBase Process { get; private set; }

        /// <summary>
        /// The method launches a browser instance with given arguments. The browser will be closed when the Browser is disposed.
        /// </summary>
        /// <param name="options">Options for launching the browser.</param>
        /// <returns>A connected browser.</returns>
        /// <remarks>
        /// See <a href="https://www.howtogeek.com/202825/what%E2%80%99s-the-difference-between-chromium-and-chrome/">this article</a>
        /// for a description of the differences between Chromium and Chrome.
        /// <a href="https://chromium.googlesource.com/chromium/src/+/lkcr/docs/chromium_browser_vs_google_chrome.md">This article</a> describes some differences for Linux users.
        /// </remarks>
        public async Task<IBrowser> LaunchAsync(LaunchOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            EnsureSingleLaunchOrConnect();
            _browser = options.Browser;
            var buildId = options.Browser switch
            {
                SupportedBrowser.Firefox => await Firefox.GetDefaultBuildIdAsync().ConfigureAwait(false),
                SupportedBrowser.Chrome or SupportedBrowser.ChromeHeadlessShell => Chrome.DefaultBuildId,
                SupportedBrowser.Chromium => await Chromium.ResolveBuildIdAsync(BrowserFetcher.GetCurrentPlatform()).ConfigureAwait(false),
                _ => throw new ArgumentException("Invalid browser"),
            };
            var executable = options.ExecutablePath ?? GetExecutablePath(options, buildId);

            Process = options.Browser switch
            {
                SupportedBrowser.Chrome or SupportedBrowser.Chromium => new ChromeLauncher(executable, options),
                SupportedBrowser.Firefox => new FirefoxLauncher(executable, options),
                _ => throw new ArgumentException("Invalid product"),
            };

            try
            {
                await Process.StartAsync().ConfigureAwait(false);

                Connection connection = null;
                try
                {
                    connection = await Connection
                        .Create(Process.EndPoint, options, _loggerFactory)
                        .ConfigureAwait(false);

                    var browser = await CdpBrowser
                        .CreateAsync(
                            options.Browser,
                            connection,
                            [],
                            options.IgnoreHTTPSErrors,
                            options.DefaultViewport,
                            Process,
                            options.TargetFilter,
                            options.IsPageTarget)
                        .ConfigureAwait(false);

                    await browser.WaitForTargetAsync(t => t.Type == TargetType.Page).ConfigureAwait(false);
                    return browser;
                }
                catch (Exception ex)
                {
                    connection?.Dispose();
                    throw new ProcessException("Failed to create connection", ex);
                }
            }
            catch
            {
                await Process.KillAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Attaches Puppeteer to an existing process instance. The browser will be closed when the Browser is disposed.
        /// </summary>
        /// <param name="options">Options for connecting.</param>
        /// <returns>A connected browser.</returns>
        public async Task<IBrowser> ConnectAsync(ConnectOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            EnsureSingleLaunchOrConnect();

            if (!string.IsNullOrEmpty(options.BrowserURL) && !string.IsNullOrEmpty(options.BrowserWSEndpoint))
            {
                throw new PuppeteerException("Exactly one of browserWSEndpoint or browserURL must be passed to puppeteer.connect");
            }

            Connection connection = null;
            try
            {
                var browserWSEndpoint = string.IsNullOrEmpty(options.BrowserURL)
                    ? options.BrowserWSEndpoint
                    : await GetWSEndpointAsync(options.BrowserURL).ConfigureAwait(false);

                connection = await Connection.Create(browserWSEndpoint, options, _loggerFactory).ConfigureAwait(false);

                var version = await connection.SendAsync<BrowserGetVersionResponse>("Browser.getVersion").ConfigureAwait(false);

                var product = version.Product.ToLower(CultureInfo.CurrentCulture).Contains("firefox")
                  ? SupportedBrowser.Firefox
                  : SupportedBrowser.Chromium;

                var response = await connection.SendAsync<GetBrowserContextsResponse>("Target.getBrowserContexts").ConfigureAwait(false);
                return await CdpBrowser
                    .CreateAsync(
                        product,
                        connection,
                        response.BrowserContextIds,
                        options.IgnoreHTTPSErrors,
                        options.DefaultViewport,
                        null,
                        options.TargetFilter,
                        options.IsPageTarget,
                        options.InitAction)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                connection?.Dispose();
                throw new ProcessException("Failed to create connection", ex);
            }
        }

        private async Task<string> GetWSEndpointAsync(string browserURL)
        {
            try
            {
                if (Uri.TryCreate(new Uri(browserURL), "/json/version", out var endpointURL))
                {
                    string data;
                    using (var client = new HttpClient())
                    {
                        data = await client.GetStringAsync(endpointURL).ConfigureAwait(false);
                    }

                    return JsonSerializer.Deserialize<WSEndpointResponse>(data).WebSocketDebuggerUrl;
                }

                throw new PuppeteerException($"Invalid URL {browserURL}");
            }
            catch (Exception ex)
            {
                throw new PuppeteerException($"Failed to fetch browser webSocket url from {browserURL}.", ex);
            }
        }

        private void EnsureSingleLaunchOrConnect()
        {
            if (_processLaunched)
            {
                throw new InvalidOperationException("Unable to create or connect to another process");
            }

            _processLaunched = true;
        }

        private string ResolveExecutablePath(HeadlessMode headlessMode, string buildId)
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

            return new InstalledBrowser(
                new Cache(),
                headlessMode == HeadlessMode.Shell && _browser == SupportedBrowser.Chrome ? SupportedBrowser.ChromeHeadlessShell : _browser,
                buildId,
                BrowserFetcher.GetCurrentPlatform()).GetExecutablePath();
        }

        private string GetExecutablePath(LaunchOptions options, string buildId)
        {
            if (options.Channel.HasValue)
            {
                return ComputeSystemExecutablePath(_browser, options.Channel.Value);
            }

            return ResolveExecutablePath(options.HeadlessMode, buildId);
        }

        private string ComputeSystemExecutablePath(SupportedBrowser browser, ChromeReleaseChannel channel)
            => browser switch
            {
                SupportedBrowser.Chrome => Chrome.ResolveSystemExecutablePath(BrowserFetcher.GetCurrentPlatform(), channel),
                _ => throw new PuppeteerException($"System browser detection is not supported for {browser} yet."),
            };
    }
}
