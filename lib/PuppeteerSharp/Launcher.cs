using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// Launcher controls the creation of Chromium processes or the connection remote ones.
    /// </summary>
    public class Launcher
    {
        #region Constants
        internal static readonly string[] DefaultArgs = {
            "--disable-background-networking",
            "--disable-background-timer-throttling",
            "--disable-breakpad",
            "--disable-client-side-phishing-detection",
            "--disable-default-apps",
            "--disable-dev-shm-usage",
            "--disable-extensions",
            "--disable-features=site-per-process",
            "--disable-hang-monitor",
            "--disable-popup-blocking",
            "--disable-prompt-on-repost",
            "--disable-sync",
            "--disable-translate",
            "--metrics-recording-only",
            "--no-first-run",
            "--safebrowsing-disable-auto-update"
        };
        internal static readonly string[] AutomationArgs = {
            "--enable-automation",
            "--password-store=basic",
            "--use-mock-keychain"
        };
        private const string UserDataDirArgument = "--user-data-dir";
        #endregion

        #region Private members
        private static int _processCount;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private Process _chromeProcess;
        private TempDirectory _temporaryUserDataDir;
        private Connection _connection;
        private LaunchOptions _options;
        private readonly TaskCompletionSource<bool> _chromeCloseTcs = new TaskCompletionSource<bool>();
        private bool _chromiumLaunched;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the process created by the instance is closed.
        /// </summary>
        /// <value><c>true</c> if is the process is closed; otherwise, <c>false</c>.</value>
        public bool IsChromeClosed => _chromeCloseTcs.Task.IsCompleted;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Launcher"/> class.
        /// </summary>
        /// <param name="loggerFactory">Logger factory.</param>
        public Launcher(ILoggerFactory loggerFactory = null)

        {
            _loggerFactory = loggerFactory ?? new LoggerFactory();
            _logger = _loggerFactory.CreateLogger<Launcher>();
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
            if (_chromiumLaunched)
            {
                throw new InvalidOperationException("Unable to create or connect to another chromium process");
            }
            _chromiumLaunched = true;
            var chromeArguments = InitChromeArgument(options);
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

            _chromeProcess = CreateChromeProcess(options, chromeArguments, chromeExecutable);
            try
            {
                var connectionDelay = options.SlowMo;
                var browserWSEndpoint = await WaitForEndpoint(_chromeProcess, options.Timeout).ConfigureAwait(false);

                try
                {
                    var keepAliveInterval = 0;
                    _connection = await Connection
                        .Create(browserWSEndpoint, connectionDelay, keepAliveInterval, _loggerFactory)
                        .ConfigureAwait(false);

                    var browser = await Browser.CreateAsync(
                            _connection,
                            Array.Empty<string>(),
                            options.IgnoreHTTPSErrors,
                            !options.AppMode,
                            _chromeProcess,
                            GracefullyCloseChrome
                        )
                        .ConfigureAwait(false);

                    await EnsureInitialPageAsync(browser).ConfigureAwait(false);
                    return browser;
                }
                catch (Exception ex)
                {
                    throw new ChromeProcessException("Failed to create connection", ex);
                }
            }
            catch
            {
                KillChrome();
                await _chromeCloseTcs.Task.ConfigureAwait(false);
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
            try
            {
                if (_chromiumLaunched)
                {
                    throw new InvalidOperationException("Unable to create or connect to another chromium process");
                }
                _chromiumLaunched = true;

                var connectionDelay = options.SlowMo;
                var keepAliveInterval = 0;

                _connection = await Connection.Create(options.BrowserWSEndpoint, connectionDelay, keepAliveInterval, _loggerFactory).ConfigureAwait(false);

                var response = await _connection.SendAsync<GetBrowserContextsResponse>("Target.getBrowserContexts");

                return await Browser.CreateAsync(_connection, response.BrowserContextIds, options.IgnoreHTTPSErrors, true, null, async () =>
                {
                    try
                    {
                        await _connection.SendAsync("Browser.close", null);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create connection", ex);
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

        private Process CreateChromeProcess(LaunchOptions options, List<string> chromeArguments, string chromeExecutable)
        {
            var chromeProcess = new Process
            {
                EnableRaisingEvents = true
            };
            chromeProcess.StartInfo.UseShellExecute = false;
            chromeProcess.StartInfo.FileName = chromeExecutable;
            chromeProcess.StartInfo.Arguments = string.Join(" ", chromeArguments);
            chromeProcess.StartInfo.RedirectStandardError = true;

            SetEnvVariables(chromeProcess.StartInfo.Environment, options.Env, Environment.GetEnvironmentVariables());

            chromeProcess.Exited += OnChromeProcessExited;
            if (options.DumpIO)
            {
                chromeProcess.ErrorDataReceived += OnChromeProcessErrorDataReceived;
            }

            return chromeProcess;
        }

        private void OnChromeProcessExited(object sender, EventArgs e)
        {
            if (_options.LogProcess)
            {
                _logger.LogInformation("Process Count: {ProcessCount}", Interlocked.Decrement(ref _processCount));
            }

            _chromeProcess.Exited -= OnChromeProcessExited;
            _chromeCloseTcs.TrySetResult(true);
        }

        private static void OnChromeProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.Error.WriteLine(e.Data);
        }

        private List<string> InitChromeArgument(LaunchOptions options)
        {
            var chromeArguments = new List<string>(DefaultArgs);

            _options = options;

            if (options.AppMode)
            {
                options.Headless = false;
            }
            else
            {
                chromeArguments.AddRange(AutomationArgs);
            }

            if (!options.IgnoreDefaultArgs ||
                !chromeArguments.Any(argument => argument.StartsWith("--remote-debugging-", StringComparison.Ordinal)))
            {
                chromeArguments.Add("--remote-debugging-port=0");
            }

            var userDataDirOption = options.Args.FirstOrDefault(i => i.StartsWith(UserDataDirArgument, StringComparison.Ordinal));
            if (string.IsNullOrEmpty(userDataDirOption))
            {
                if (string.IsNullOrEmpty(options.UserDataDir))
                {
                    _temporaryUserDataDir = new TempDirectory();
                    chromeArguments.Add($"{UserDataDirArgument}={_temporaryUserDataDir.Path.Quote()}");
                }
                else
                {
                    chromeArguments.Add($"{UserDataDirArgument}={options.UserDataDir.Quote()}");
                }
            }
            else
            {
                _options.UserDataDir = userDataDirOption.Replace($"{UserDataDirArgument}=", string.Empty).UnQuote();
            }

            if (options.Devtools)
            {
                chromeArguments.Add("--auto-open-devtools-for-tabs");
                options.Headless = false;
            }

            if (options.Headless)
            {
                chromeArguments.AddRange(new[]{
                    "--headless",
                    "--disable-gpu",
                    "--hide-scrollbars",
                    "--mute-audio"
                });
            }

            if (!options.IgnoreDefaultArgs && options.Args.Any() && options.Args.All(arg => arg.StartsWith("-")))
            {
                chromeArguments.Add("about:blank");
            }

            if (options.Args.Any())
            {
                chromeArguments.AddRange(options.Args);
            }

            return chromeArguments;
        }

        private async Task<string> WaitForEndpoint(Process chromeProcess, int timeout)
        {
            var chromeStartTcs = new TaskCompletionSource<string>();
            var output = new StringBuilder();

            void OnChromeExited(object sender, EventArgs e)
            {
                chromeStartTcs.SetException(new ChromeProcessException($"Failed to launch chrome! {output}"));
            }
            void OnChromeDataReceived(object sender, DataReceivedEventArgs e)
            {
                if (e.Data != null)
                {
                    output.AppendLine(e.Data);
                    var match = Regex.Match(e.Data, "^DevTools listening on (ws:\\/\\/.*)");
                    if (match.Success)
                    {
                        chromeStartTcs.SetResult(match.Groups[1].Value);
                    }
                }
            }

            chromeProcess.ErrorDataReceived += OnChromeDataReceived;
            chromeProcess.Exited += OnChromeExited;
            CancellationTokenSource cts = null;
            try
            {
                chromeProcess.Start();
                if (_options.LogProcess)
                {
                    _logger.LogInformation("Process Count: {ProcessCount}", Interlocked.Increment(ref _processCount));
                }

                chromeProcess.BeginErrorReadLine();
                if (timeout > 0)
                {
                    cts = new CancellationTokenSource(timeout);
                    cts.Token.Register(() => chromeStartTcs.TrySetException(
                        new ChromeProcessException($"Timed out after {timeout} ms while trying to connect to Chrome!")));
                }

                await chromeStartTcs.Task.ConfigureAwait(false);
            }
            finally
            {
                cts?.Dispose();
                chromeProcess.Exited -= OnChromeExited;
                chromeProcess.ErrorDataReceived -= OnChromeDataReceived;
            }

            return chromeStartTcs.Task.Result;
        }

        private async Task GracefullyCloseChrome()
        {
            if (_connection != null)
            {
                try
                {
                    await _connection.SendAsync("Browser.close", null).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    KillChrome();
                }
            }
            else
            {
                KillChrome();
            }

            await _chromeCloseTcs.Task.ConfigureAwait(false);

            _temporaryUserDataDir?.Dispose();
        }

        private void KillChrome()
        {
            try
            {
                if (!_chromeProcess.HasExited)
                {
                    _chromeProcess.Kill();
                }
            }
            catch (InvalidOperationException)
            {
                // Ignore
            }
        }

        private static void SetEnvVariables(IDictionary<string, string> environment, IDictionary<string, string> customEnv,
                                            IDictionary realEnv)
        {
            foreach (DictionaryEntry item in realEnv)
            {
                environment[item.Key.ToString()] = item.Value.ToString();
            }

            if (customEnv != null)
            {
                foreach (var item in customEnv)
                {
                    environment[item.Key] = item.Value;
                }
            }
        }

        #endregion
    }
}
