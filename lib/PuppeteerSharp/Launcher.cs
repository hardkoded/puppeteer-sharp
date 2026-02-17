using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
#if !CDP_ONLY
using PuppeteerSharp.Bidi;
#endif
using PuppeteerSharp.BrowserData;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers.Json;
#if !CDP_ONLY
using WebDriverBiDi;
using BidiTransport = WebDriverBiDi.Protocol.Transport;
#endif
using CdpConnection = PuppeteerSharp.Cdp.Connection;

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
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The caller is responsible for disposing the returned object.")]
        public async Task<IBrowser> LaunchAsync(LaunchOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            EnsureSingleLaunchOrConnect();
            _browser = options.Browser;

            if (options.Browser == SupportedBrowser.Firefox)
            {
                options.Protocol = ProtocolType.WebdriverBiDi;
            }

            var executable = options.ExecutablePath;
            if (executable == null)
            {
                var buildId = options.Browser switch
                {
                    SupportedBrowser.Firefox => await Firefox.GetDefaultBuildIdAsync().ConfigureAwait(false),
                    SupportedBrowser.Chrome or SupportedBrowser.ChromeHeadlessShell => Chrome.DefaultBuildId,
                    SupportedBrowser.Chromium => await Chromium.ResolveBuildIdAsync(BrowserFetcher.GetCurrentPlatform()).ConfigureAwait(false),
                    _ => throw new ArgumentException("Invalid browser"),
                };
                executable = GetExecutablePath(options, buildId);
            }

            Process = options.Browser switch
            {
                SupportedBrowser.Chrome or SupportedBrowser.Chromium or SupportedBrowser.ChromeHeadlessShell => new ChromeLauncher(executable, options),
                SupportedBrowser.Firefox => new FirefoxLauncher(executable, options),
                _ => throw new ArgumentException("Invalid browser"),
            };

            try
            {
                if (options.Protocol == ProtocolType.WebdriverBiDi)
                {
                    Process.StateManager.LineOutputExpression = "^WebDriver BiDi listening on (ws:\\/\\/.*)$";
                }

                await Process.StartAsync().ConfigureAwait(false);

                CdpConnection connection = null;
                IBrowser browser = null;

                try
                {
                    if (options.Protocol == ProtocolType.WebdriverBiDi)
                    {
#if !CDP_ONLY
                        var driver = await CreateBidiDriverAsync(Process.EndPoint + "/session", options).ConfigureAwait(false);
                        browser = await BidiBrowser.CreateAsync(driver, options, _loggerFactory, Process).ConfigureAwait(false);
#else
                        throw new ArgumentException("Invalid browser. Only CDP is supported");
#endif
                    }
                    else
                    {
                        connection = await CdpConnection
                            .Create(Process.EndPoint, options, _loggerFactory)
                            .ConfigureAwait(false);

                        browser = await CdpBrowser
                            .CreateAsync(
                                options.Browser,
                                connection,
                                [],
                                options.AcceptInsecureCerts,
                                options.DefaultViewport,
                                Process,
                                options.TargetFilter,
                                options.IsPageTarget)
                            .ConfigureAwait(false);
                    }

                    if (options.WaitForInitialPage)
                    {
                        await browser.WaitForTargetAsync(t => t.Type == TargetType.Page).ConfigureAwait(false);
                    }

                    return browser;
                }
                catch (Exception ex)
                {
                    connection?.Dispose();
                    browser?.Dispose();
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

            var browserWSEndpoint = string.IsNullOrEmpty(options.BrowserURL)
                ? options.BrowserWSEndpoint
                : await GetWSEndpointAsync(options.BrowserURL).ConfigureAwait(false);

            if (options.Protocol == ProtocolType.WebdriverBiDi)
            {
#if !CDP_ONLY
                return await ConnectBidiAsync(browserWSEndpoint, options).ConfigureAwait(false);
#else
                throw new ArgumentException("Invalid browser. Only CDP is supported");
#endif
            }

            return await ConnectCdpAsync(browserWSEndpoint, options).ConfigureAwait(false);
        }

#if !CDP_ONLY
        private static async Task<BiDiDriver> CreateBidiDriverAsync(string browserWSEndpoint, IConnectionOptions options)
        {
            if (options.TransportFactory != null)
            {
                var transport = await options.TransportFactory(new Uri(browserWSEndpoint), options, CancellationToken.None).ConfigureAwait(false);
                BiDiDriver driver = null;
                try
                {
                    var puppeteerConnection = new PuppeteerConnection(transport);
                    var bidiTransport = new BidiTransport(puppeteerConnection);
                    driver = new BiDiDriver(TimeSpan.FromMilliseconds(options.ProtocolTimeout), bidiTransport);
                    await driver.StartAsync(browserWSEndpoint).ConfigureAwait(false);
                    return driver;
                }
                catch
                {
                    if (driver != null)
                    {
                        await driver.StopAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        transport.Dispose();
                    }

                    throw;
                }
            }

            var defaultDriver = new BiDiDriver(TimeSpan.FromMilliseconds(options.ProtocolTimeout));
            await defaultDriver.StartAsync(browserWSEndpoint).ConfigureAwait(false);
            return defaultDriver;
        }

        private async Task<IBrowser> ConnectBidiAsync(string browserWSEndpoint, ConnectOptions options)
        {
            BiDiDriver driver = null;
            try
            {
                driver = await CreateBidiDriverAsync(browserWSEndpoint, options).ConfigureAwait(false);
                return await BidiBrowser.CreateAsync(driver, options, _loggerFactory, null).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (driver != null)
                {
                    await driver.StopAsync().ConfigureAwait(false);
                }

                throw new ProcessException("Failed to create connection", ex);
            }
        }
#endif

        private async Task<IBrowser> ConnectCdpAsync(string browserWSEndpoint, ConnectOptions options)
        {
            CdpConnection connection = null;
            try
            {
                connection = await CdpConnection.Create(browserWSEndpoint, options, _loggerFactory).ConfigureAwait(false);

                var version = await connection.SendAsync<BrowserGetVersionResponse>("Browser.getVersion").ConfigureAwait(false);

                var browser = version.Product.ToLower(CultureInfo.CurrentCulture).Contains("firefox")
                  ? SupportedBrowser.Firefox
                  : SupportedBrowser.Chromium;

                var response = await connection.SendAsync<GetBrowserContextsResponse>("Target.getBrowserContexts").ConfigureAwait(false);
                return await CdpBrowser
                    .CreateAsync(
                        browser,
                        connection,
                        response.BrowserContextIds,
                        options.AcceptInsecureCerts,
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

                    return JsonSerializer.Deserialize<WSEndpointResponse>(data, JsonHelper.DefaultJsonSerializerSettings.Value).WebSocketDebuggerUrl;
                }

                throw new PuppeteerException($"Invalid URL {browserURL}");
            }
            catch (Exception ex)
            {
                throw new ProcessException($"Failed to fetch browser webSocket url from {browserURL}.", ex);
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
