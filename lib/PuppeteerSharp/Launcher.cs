using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// Launcher controls the creation of processes or the connection remote ones.
    /// </summary>
    public class Launcher
    {
        private readonly ILoggerFactory _loggerFactory;
        private bool _processLaunched;
        private Product _product;

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
        /// <param name="options">Options for launching the browser</param>
        /// <returns>A connected browser.</returns>
        /// <remarks>
        /// See <a href="https://www.howtogeek.com/202825/what%E2%80%99s-the-difference-between-chromium-and-chrome/">this article</a>
        /// for a description of the differences between Chromium and Chrome.
        /// <a href="https://chromium.googlesource.com/chromium/src/+/lkcr/docs/chromium_browser_vs_google_chrome.md">This article</a> describes some differences for Linux users.
        /// </remarks>
        public async Task<Browser> LaunchAsync(LaunchOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            EnsureSingleLaunchOrConnect();
            _product = options.Product;
            var executable = await GetOrFetchBrowserExecutableAsync(options).ConfigureAwait(false);

            Process = options.Product switch
            {
                Product.Chrome => new ChromiumLauncher(executable, options),
                Product.Firefox => new FirefoxLauncher(executable, options),
                _ => throw new ArgumentException("Invalid product"),
            };

            try
            {
                await Process.StartAsync().ConfigureAwait(false);

                try
                {
                    var connection = await Connection
                        .Create(Process.EndPoint, options, _loggerFactory)
                        .ConfigureAwait(false);

                    var browser = await Browser
                        .CreateAsync(connection, Array.Empty<string>(), options.IgnoreHTTPSErrors, options.DefaultViewport, Process, options.TargetFilter)
                        .ConfigureAwait(false);

                    await browser.WaitForTargetAsync(t => t.Type == TargetType.Page).ConfigureAwait(false);
                    return browser;
                }
                catch (Exception ex)
                {
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
        public async Task<Browser> ConnectAsync(ConnectOptions options)
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

            try
            {
                var browserWSEndpoint = string.IsNullOrEmpty(options.BrowserURL)
                    ? options.BrowserWSEndpoint
                    : await GetWSEndpointAsync(options.BrowserURL).ConfigureAwait(false);

                var connection = await Connection.Create(browserWSEndpoint, options, _loggerFactory).ConfigureAwait(false);
                var response = await connection.SendAsync<GetBrowserContextsResponse>("Target.getBrowserContexts").ConfigureAwait(false);
                return await Browser
                    .CreateAsync(
                        connection,
                        response.BrowserContextIds,
                        options.IgnoreHTTPSErrors,
                        options.DefaultViewport,
                        null,
                        options.TargetFilter,
                        options.InitAction)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
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

                    return JsonConvert.DeserializeObject<WSEndpointResponse>(data).WebSocketDebuggerUrl;
                }

                throw new PuppeteerException($"Invalid URL {browserURL}");
            }
            catch (Exception ex)
            {
                throw new PuppeteerException($"Failed to fetch browser webSocket url from {browserURL}.", ex);
            }
        }

        /// <summary>
        /// Gets the executable path.
        /// </summary>
        /// <returns>The executable path.</returns>
        public Task<string> GetExecutablePathAsync() => ResolveExecutablePathAsync();

        private void EnsureSingleLaunchOrConnect()
        {
            if (_processLaunched)
            {
                throw new InvalidOperationException("Unable to create or connect to another process");
            }

            _processLaunched = true;
        }

        private async Task<string> GetOrFetchBrowserExecutableAsync(LaunchOptions options)
        {
            var browserExecutable = options.ExecutablePath;

            if (string.IsNullOrEmpty(browserExecutable))
            {
                browserExecutable = await ResolveExecutablePathAsync().ConfigureAwait(false);
            }

            if (!File.Exists(browserExecutable))
            {
                throw new FileNotFoundException("Failed to launch browser! path to executable does not exist", browserExecutable);
            }
            return browserExecutable;
        }

        private async Task<string> ResolveExecutablePathAsync()
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

            var revision = Environment.GetEnvironmentVariable("PUPPETEER_CHROMIUM_REVISION");
            using var browserFetcher = new BrowserFetcher(_product);
            RevisionInfo revisionInfo;

            if (!string.IsNullOrEmpty(revision))
            {
                revisionInfo = browserFetcher.RevisionInfo(revision);
                if (!revisionInfo.Local)
                {
                    throw new FileNotFoundException("Tried to use PUPPETEER_CHROMIUM_REVISION env variable to launch browser but did not find executable", revisionInfo.ExecutablePath);
                }
                return revisionInfo.ExecutablePath;
            }
            revisionInfo = await browserFetcher.GetRevisionInfoAsync().ConfigureAwait(false);
            if (!revisionInfo.Local)
            {
                throw new FileNotFoundException("Process revision is not downloaded. Run BrowserFetcher.DownloadAsync or download the process manually", revisionInfo.ExecutablePath);
            }
            return revisionInfo.ExecutablePath;
        }
    }
}
