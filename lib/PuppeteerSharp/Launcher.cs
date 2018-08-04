using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections;
using Microsoft.Extensions.Logging;

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
        private string _temporaryUserDataDir;
        private Connection _connection;
        private Timer _timer;
        private LaunchOptions _options;
        private TaskCompletionSource<bool> _waitForChromeToClose;
        private bool _processLoaded;
        private bool _chromiumLaunched;
        private object _isChromeCloseLock = new object();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the process created by the instance is closed.
        /// </summary>
        /// <value><c>true</c> if is the process is closed; otherwise, <c>false</c>.</value>
        public bool IsChromeClosed { get; internal set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Launcher"/> class.
        /// </summary>
        /// <param name="loggerFactory">Logger factory.</param>
        public Launcher(ILoggerFactory loggerFactory = null)

        {
            _loggerFactory = loggerFactory ?? new LoggerFactory();
            _logger = _loggerFactory.CreateLogger<Launcher>();
            _waitForChromeToClose = new TaskCompletionSource<bool>();
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

            CreateChromeProcess(options, chromeArguments, chromeExecutable);

            try
            {
                var connectionDelay = options.SlowMo;
                var browserWSEndpoint = await WaitForEndpoint(_chromeProcess, options.Timeout).ConfigureAwait(false);
                var keepAliveInterval = 0;

                _connection = await Connection.Create(browserWSEndpoint, connectionDelay, keepAliveInterval, _loggerFactory).ConfigureAwait(false);
                _processLoaded = true;

                if (options.LogProcess)
                {
                    _logger.LogInformation("Process Count: {ProcessCount}", Interlocked.Increment(ref _processCount));
                }

                var browser = await Browser.CreateAsync(_connection, options, _chromeProcess, GracefullyCloseChrome).ConfigureAwait(false);
                await EnsureInitialPageAsync(browser).ConfigureAwait(false);
                return browser;
            }
            catch (Exception ex)
            {
                KillChrome();
                throw new ChromeProcessException("Failed to create connection", ex);
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

                return await Browser.CreateAsync(_connection, options, null, () =>
                {
                    try
                    {
                        var closeTask = _connection.SendAsync("Browser.close", null);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                    return null;
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create connection", ex);
            }
        }

        /// <summary>
        /// Tries the delete user data dir.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="times">How many times it should try to delete the folder</param>
        /// <param name="delay">Time to wait between tries.</param>
        public async Task TryDeleteUserDataDir(int times = 10, TimeSpan? delay = null)
        {
            if (!IsChromeClosed)
            {
                throw new InvalidOperationException("Unable to delete user data dir, Chorme is still open");
            }

            if (times <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(times));
            }

            if (delay == null)
            {
                delay = new TimeSpan(0, 0, 0, 0, 100);
            }

            var folder = string.IsNullOrEmpty(_temporaryUserDataDir) ? _options.UserDataDir : _temporaryUserDataDir;
            var attempts = 0;
            while (true)
            {
                try
                {
                    attempts++;
                    Directory.Delete(folder, true);
                    break;
                }
                catch (UnauthorizedAccessException)
                {
                    if (attempts == times)
                    {
                        throw;
                    }

                    await Task.Delay(delay.Value).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Gets the executable path.
        /// </summary>
        /// <returns>The executable path.</returns>
        public static string GetExecutablePath()
            => new BrowserFetcher().RevisionInfo(BrowserFetcher.DefaultRevision).ExecutablePath;

        /// <summary>
        /// Gets a temporary directory using <see cref="Path.GetTempPath"/> and <see cref="Path.GetRandomFileName"/>.
        /// </summary>
        /// <returns>A temporary directory.</returns>
        public static string GetTemporaryDirectory()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

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

        private void CreateChromeProcess(LaunchOptions options, List<string> chromeArguments, string chromeExecutable)
        {
            _chromeProcess = new Process
            {
                EnableRaisingEvents = true
            };
            _chromeProcess.StartInfo.UseShellExecute = false;
            _chromeProcess.StartInfo.FileName = chromeExecutable;
            _chromeProcess.StartInfo.Arguments = string.Join(" ", chromeArguments);
            _chromeProcess.StartInfo.RedirectStandardError = true;

            SetEnvVariables(_chromeProcess.StartInfo.Environment, options.Env, Environment.GetEnvironmentVariables());

            _chromeProcess.Exited += async (sender, e) =>
            {
                await AfterProcessExit().ConfigureAwait(false);
            };

            _chromeProcess.ErrorDataReceived += (sender, e) =>
            {
                if (options.DumpIO)
                {
                    Console.Error.WriteLine(e.Data);
                }
            };
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
                    _temporaryUserDataDir = GetTemporaryDirectory();
                    chromeArguments.Add($"{UserDataDirArgument}={_temporaryUserDataDir.Quote()}");
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

        private Task<string> WaitForEndpoint(Process chromeProcess, int timeout)
        {
            var taskWrapper = new TaskCompletionSource<string>();
            var output = string.Empty;

            void exitedEvent(object sender, EventArgs e)
            {
                if (_options.LogProcess && !_processLoaded)
                {
                    _logger.LogInformation("Process Count: {ProcessCount}", Interlocked.Increment(ref _processCount));
                }

                CleanUp();

                taskWrapper.SetException(new ChromeProcessException($"Failed to launch chrome! {output}"));
            }

            void errorDataReceivedEvent(object sender, DataReceivedEventArgs e)
            {
                if (e.Data != null)
                {
                    output += e.Data + "\n";
                    var match = Regex.Match(e.Data, "^DevTools listening on (ws:\\/\\/.*)");

                    if (!match.Success)
                    {
                        return;
                    }

                    CleanUp();
                    chromeProcess.Exited -= exitedEvent;
                    chromeProcess.ErrorDataReceived -= errorDataReceivedEvent;
                    taskWrapper.SetResult(match.Groups[1].Value);
                }
            }

            chromeProcess.ErrorDataReceived += errorDataReceivedEvent;
            chromeProcess.Exited += exitedEvent;

            if (timeout > 0)
            {
                //We have to declare timer before initializing it because if we don't do this 
                //we can't dispose it in the action created in the constructor
                _timer = new Timer((state) =>
                {
                    taskWrapper.SetException(
                        new ChromeProcessException($"Timed out after {timeout} ms while trying to connect to Chrome! "));
                    _timer.Dispose();
                }, null, timeout, 0);
            }

            chromeProcess.Start();
            chromeProcess.BeginErrorReadLine();
            return taskWrapper.Task;
        }

        private void CleanUp()
        {
            _timer?.Dispose();
            _timer = null;
            _chromeProcess?.RemoveExitedEvent();
        }

        private async Task AfterProcessExit()
        {
            lock (_isChromeCloseLock)
            {
                if (IsChromeClosed)
                {
                    return;
                }
                IsChromeClosed = true;
            }
            if (_options.LogProcess)
            {
                _logger.LogInformation("Process Count: {ProcessCount}", Interlocked.Decrement(ref _processCount));
            }

            if (_temporaryUserDataDir != null)
            {
                await TryDeleteUserDataDir().ConfigureAwait(false);
            }

            if (_waitForChromeToClose.Task.Status != TaskStatus.RanToCompletion)
            {
                _waitForChromeToClose.SetResult(true);
            }
        }

        private async Task GracefullyCloseChrome()
        {
            if (!string.IsNullOrEmpty(_temporaryUserDataDir))
            {
                KillChrome();
                await AfterProcessExit().ConfigureAwait(false);
            }
            else if (_connection != null)
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

            await _waitForChromeToClose.Task.ConfigureAwait(false);
        }

        private void KillChrome()
        {
            try
            {
                if (_chromeProcess.Id != 0 && !_chromeProcess.HasExited && Process.GetProcessById(_chromeProcess.Id) != null)
                {
                    _chromeProcess.Kill();
                    _chromeProcess.WaitForExit();
                }
            }
            catch (InvalidOperationException ex) when (ex.Message == "No process is associated with this object.")
            {
                // swallow
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
