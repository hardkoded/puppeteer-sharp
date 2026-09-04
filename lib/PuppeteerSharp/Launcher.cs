using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
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
using PuppeteerSharp.Helpers;
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
                if (options.Pipe)
                {
                    throw new ArgumentException("Pipe transport is not supported for Firefox.");
                }

                options.Protocol = ProtocolType.WebdriverBiDi;
            }

            UrlRestrictionsValidator.AssertSupportedUrlRestrictions(options.Protocol, options.BlockList, options.Allowlist);

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
                // Chrome doesn't have native BiDi; it only outputs a CDP endpoint.
                // Firefox outputs a native BiDi endpoint.
                if (options.Protocol == ProtocolType.WebdriverBiDi && Process is FirefoxLauncher)
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
                        BiDiDriver driver;

                        if (Process is FirefoxLauncher)
                        {
                            // Firefox has native BiDi support
                            driver = await CreateBidiDriverAsync(Process.EndPoint + "/session", options).ConfigureAwait(false);
                        }
                        else
                        {
                            // Chrome: bridge BiDi over CDP using the chromium-bidi mapper
                            var bidiOverCdpTransport = await BidiOverCdpTransport.CreateAsync(
                                Process.EndPoint, options, _loggerFactory).ConfigureAwait(false);
                            driver = await CreateBidiDriverAsync(bidiOverCdpTransport, options).ConfigureAwait(false);
                        }

                        var bidiProcess = Process;
                        Func<Task> bidiCloseCallback = async () =>
                        {
                            try
                            {
                                var closeTimeout = TimeSpan.FromMilliseconds(5000);
                                await bidiProcess.EnsureExitAsync(closeTimeout).ConfigureAwait(false);
                            }
                            catch
                            {
                                await bidiProcess.KillAsync().ConfigureAwait(false);
                            }
                        };
                        browser = await BidiBrowser.CreateAsync(driver, options, _loggerFactory, Process, bidiCloseCallback).ConfigureAwait(false);
#else
                        throw new ArgumentException("Invalid browser. Only CDP is supported");
#endif
                    }
                    else
                    {
                        if (options.Pipe && Process is ChromeLauncher chromeLauncher)
                        {
                            chromeLauncher.InitializePipeTransport();
                            connection = CdpConnection.CreateFromTransport(chromeLauncher.PipeTransport, options, _loggerFactory);
                        }
                        else
                        {
                            connection = await CdpConnection
                                .Create(Process.EndPoint, options, _loggerFactory)
                                .ConfigureAwait(false);
                        }

                        var cdpProcess = Process;
                        Func<Task> cdpCloseCallback = async () =>
                        {
                            try
                            {
                                var closeTimeout = TimeSpan.FromMilliseconds(5000);
                                await cdpProcess.EnsureExitAsync(closeTimeout).ConfigureAwait(false);
                            }
                            catch
                            {
                                await cdpProcess.KillAsync().ConfigureAwait(false);
                            }
                        };

                        browser = await CdpBrowser
                            .CreateAsync(
                                options.Browser,
                                connection,
                                [],
                                options.AcceptInsecureCerts,
                                options.DefaultViewport,
                                Process,
                                cdpCloseCallback,
                                options.TargetFilter,
                                options.IsPageTarget,
                                handleDevToolsAsPage: options.HandleDevToolsAsPage,
                                networkEnabled: options.NetworkEnabled,
                                issuesEnabled: options.IssuesEnabled,
                                blockList: options.BlockList,
                                allowList: options.Allowlist)
                            .ConfigureAwait(false);
                    }

                    if (options.EnableExtensions is { Paths: { } extensionPaths })
                    {
                        var extensionsEnabledInIncognito = options.ExtensionsEnabledInIncognito ?? [];
                        await Task.WhenAll(
                            extensionPaths.Select(path => browser.InstallExtensionAsync(
                                path,
                                new ExtensionInstallOptions { EnabledInIncognito = extensionsEnabledInIncognito.Contains(path) }))).ConfigureAwait(false);
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

                    var userDataDir = options.UserDataDir ?? Process.TempUserDataDir?.Path;
                    if (userDataDir != null)
                    {
                        // Pipe mode does not wait on the DevTools endpoint, so stderr
                        // ProcessSingleton lines can arrive slightly after the connection
                        // failure. Wait briefly for the process to exit / flush logs.
                        await Task.WhenAny(
                            Process.ExitCompletionSource.Task,
                            Task.Delay(500)).ConfigureAwait(false);
                        ThrowIfBrowserAlreadyRunning(ex, userDataDir, Process);
                    }

                    throw new ProcessException("Failed to create connection", ex);
                }
            }
            catch (Exception ex)
            {
                await Process.KillAsync().ConfigureAwait(false);
                await Process.CleanTempUserDataDirAsync().ConfigureAwait(false);

                var userDataDir = options.UserDataDir ?? Process.TempUserDataDir?.Path;
                if (userDataDir != null)
                {
                    ThrowIfBrowserAlreadyRunning(ex, userDataDir, Process);
                }

                if (IsMissingXServer(ex, Process) && options.HeadlessMode == HeadlessMode.False)
                {
                    throw new ProcessException(
                        "Missing X server to start the headful browser. Either set Headless to true or use xvfb-run to run your Puppeteer script.",
                        ex);
                }

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

            UrlRestrictionsValidator.AssertSupportedUrlRestrictions(options.Protocol, options.BlockList, options.Allowlist);

            var browserWSEndpoint = string.IsNullOrEmpty(options.BrowserURL)
                ? options.BrowserWSEndpoint
                : await GetWSEndpointAsync(options.BrowserURL, ConnectionOptionsHelper.GetEffectiveHeaders(options)).ConfigureAwait(false);

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

        /// <summary>
        /// Returns a path to a system-wide Chrome installation for the given release channel.
        /// </summary>
        /// <param name="browser">Browser to resolve. Only <see cref="SupportedBrowser.Chrome"/> is supported.</param>
        /// <param name="channel">Release channel to look for on the system.</param>
        /// <param name="validatePath">
        /// If <c>true</c> (default), throws when no candidate exists.
        /// If <c>false</c>, returns the first resolved candidate even when the file is missing.
        /// </param>
        /// <param name="platform">
        /// Platform whose known install locations should be used.
        /// Defaults to the current platform.
        /// </param>
        /// <returns>The first existing candidate, or the first resolved path when <paramref name="validatePath"/> is <c>false</c>.</returns>
        internal static string ComputeSystemExecutablePath(
            SupportedBrowser browser,
            ChromeReleaseChannel channel,
            bool validatePath = true,
            Platform? platform = null)
        {
            if (browser != SupportedBrowser.Chrome)
            {
                throw new PuppeteerException($"System browser detection is not supported for {browser} yet.");
            }

            var paths = Chrome.ResolveSystemExecutablePaths(platform ?? BrowserFetcher.GetCurrentPlatform(), channel);

            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            if (!validatePath)
            {
                return paths[0];
            }

            throw new PuppeteerException(
                $"Could not find Google Chrome executable for channel '{channel}' at:\n - {string.Join("\n - ", paths)}");
        }

        private static void ThrowIfBrowserAlreadyRunning(Exception ex, string userDataDir, LauncherBase process)
        {
            if (!IsBrowserAlreadyRunning(ex, userDataDir, process))
            {
                return;
            }

            // The browser reports the same ProcessSingleton failure whether another
            // instance holds the lock or it simply cannot write to the profile
            // directory, so check for the latter before blaming a running browser.
            if (!IsWritableDirectory(userDataDir))
            {
                throw new ProcessException(
                    $"The browser cannot write to {userDataDir}. Make the UserDataDir writable or use a different one.",
                    ex);
            }

            throw new ProcessException(
                $"The browser is already running for {userDataDir}. Use a different UserDataDir or stop the running browser first.",
                ex);
        }

        private static bool IsBrowserAlreadyRunning(Exception ex, string userDataDir, LauncherBase process)
        {
            // Prefer recent stderr logs (available even under Pipe=true) over the
            // exception text, matching upstream browserProcess.getRecentLogs().
            var logs = process?.GetRecentLogs() ?? string.Empty;
            var message = string.IsNullOrEmpty(logs) ? ex.ToString() : logs + "\n" + ex;
            if (message.Contains("Failed to create a ProcessSingleton for your profile directory", StringComparison.Ordinal))
            {
                return true;
            }

            // On Windows we will not get logs due to the singleton process
            // handover. See
            // https://source.chromium.org/chromium/chromium/src/+/main:chrome/browser/process_singleton_win.cc;l=46;drc=fc7952f0422b5073515a205a04ec9c3a1ae81658
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                File.Exists(Path.Combine(userDataDir, "lockfile")))
            {
                return true;
            }

            return false;
        }

        private static bool IsWritableDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return true;
            }

            try
            {
                var testPath = Path.Combine(directory, $".puppeteer-write-test-{Guid.NewGuid():N}");
                File.WriteAllText(testPath, string.Empty);
                File.Delete(testPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsMissingXServer(Exception ex, LauncherBase process)
        {
            var logs = process?.GetRecentLogs() ?? string.Empty;
            var message = string.IsNullOrEmpty(logs) ? ex.ToString() : logs + "\n" + ex;
            return message.Contains("Missing X server", StringComparison.Ordinal);
        }

#if !CDP_ONLY
        private static async Task<BiDiDriver> CreateBidiDriverAsync(BidiOverCdpTransport transport, IConnectionOptions options)
        {
            PuppeteerConnection puppeteerConnection = null;
            BidiTransport bidiTransport = null;
            BiDiDriver driver = null;
            try
            {
#pragma warning disable CA2000 // Ownership is transferred to the driver
                puppeteerConnection = new PuppeteerConnection(transport);
                bidiTransport = new BidiTransport(puppeteerConnection);
#pragma warning restore CA2000
                driver = new BiDiDriver(TimeSpan.FromMilliseconds(options.ProtocolTimeout), bidiTransport);
                await driver.StartAsync("bidi-over-cdp://local").ConfigureAwait(false);
                return driver;
            }
            catch
            {
                if (driver != null)
                {
                    await driver.StopAsync().ConfigureAwait(false);
                }
                else if (bidiTransport != null)
                {
                    await bidiTransport.DisposeAsync().ConfigureAwait(false);
                }
                else if (puppeteerConnection != null)
                {
                    await puppeteerConnection.DisposeAsync().ConfigureAwait(false);
                }
                else
                {
                    transport.Dispose();
                }

                throw;
            }
        }

        private static async Task<BiDiDriver> CreateBidiDriverAsync(string browserWSEndpoint, IConnectionOptions options)
        {
            if (options.TransportFactory != null)
            {
                var transport = await options.TransportFactory(new Uri(browserWSEndpoint), options, CancellationToken.None).ConfigureAwait(false);
                PuppeteerConnection puppeteerConnection = null;
                BidiTransport bidiTransport = null;
                BiDiDriver driver = null;
                try
                {
#pragma warning disable CA2000 // Ownership is transferred to the driver
                    puppeteerConnection = new PuppeteerConnection(transport);
                    bidiTransport = new BidiTransport(puppeteerConnection);
#pragma warning restore CA2000
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
                    else if (bidiTransport != null)
                    {
                        await bidiTransport.DisposeAsync().ConfigureAwait(false);
                    }
                    else if (puppeteerConnection != null)
                    {
                        await puppeteerConnection.DisposeAsync().ConfigureAwait(false);
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
                        closeCallback: null,
                        options.TargetFilter,
                        options.IsPageTarget,
                        options.InitAction,
                        options.HandleDevToolsAsPage,
                        options.NetworkEnabled,
                        options.IssuesEnabled,
                        options.BlockList,
                        options.Allowlist)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                connection?.Dispose();
                throw new ProcessException("Failed to create connection", ex);
            }
        }

        private async Task<string> GetWSEndpointAsync(string browserURL, Dictionary<string, string> headers)
        {
            try
            {
                if (Uri.TryCreate(new Uri(browserURL), "/json/version", out var endpointURL))
                {
                    string data;
                    using (var client = new HttpClient())
                    {
                        using var request = new HttpRequestMessage(HttpMethod.Get, endpointURL);
                        if (headers != null)
                        {
                            foreach (var header in headers)
                            {
                                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                            }
                        }

                        using var response = await client.SendAsync(request).ConfigureAwait(false);
                        response.EnsureSuccessStatusCode();
                        data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
    }
}
