using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a Chrome process and any associated temporary user data directory that have created
    /// by Puppeteer and therefore must be cleaned up when no longer needed.
    /// </summary>
    public class ChromeProcess : ChromiumProcess
    {
        #region Instance fields

        private readonly TempDirectory _tempUserDataDir;
        private readonly TaskCompletionSource<string> _startCompletionSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource<bool> _exitCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private State _currentState = State.Initial;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="ChromeProcess"/> instance.
        /// </summary>
        /// <param name="chromeExecutable">Full path of Chrome executable.</param>
        /// <param name="options">Options for launching Chrome.</param>
        /// <param name="loggerFactory">Logger factory</param>
        public ChromeProcess(string chromeExecutable, LaunchOptions options, ILoggerFactory loggerFactory)
        {
            _options = options;
            _logger = options.LogProcess
                ? loggerFactory.CreateLogger<ChromeProcess>()
                : null;

            List<string> chromeArgs;
            (chromeArgs, _tempUserDataDir) = PrepareChromeArgs(options);

            Process = new Process
            {
                EnableRaisingEvents = true
            };
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.FileName = chromeExecutable;
            Process.StartInfo.Arguments = string.Join(" ", chromeArgs);
            Process.StartInfo.RedirectStandardError = true;

            SetEnvVariables(Process.StartInfo.Environment, options.Env, Environment.GetEnvironmentVariables());

            if (options.DumpIO)
            {
                Process.ErrorDataReceived += (sender, e) => Console.Error.WriteLine(e.Data);
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~ChromeProcess()
        {
            Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets Chrome process details.
        /// </summary>
        public Process Process { get; }

        /// <summary>
        /// Gets Chrome endpoint.
        /// </summary>
        public string EndPoint => _startCompletionSource.Task.IsCompleted
            ? _startCompletionSource.Task.Result
            : null;

        /// <summary>
        /// Indicates whether Chrome process is exiting.
        /// </summary>
        public bool IsExiting => _currentState.IsExiting;

        /// <summary>
        /// Indicates whether Chrome process has exited.
        /// </summary>
        public bool HasExited => _currentState.IsExited;

        #endregion

        #region Public methods

        /// <summary>
        /// Asynchronously starts Chrome process.
        /// </summary>
        /// <returns></returns>
        public Task StartAsync() => _currentState.StartAsync(this);

        /// <summary>
        /// Asynchronously waits for graceful Chrome process exit within a given timeout period.
        /// Kills the Chrome process if it has not exited within this period.
        /// </summary>
        /// <param name="timeout">The maximum waiting time for a graceful process exit.</param>
        /// <returns></returns>
        public Task EnsureExitAsync(TimeSpan? timeout) => timeout.HasValue
            ? _currentState.ExitAsync(this, timeout.Value)
            : _currentState.KillAsync(this);

        /// <summary>
        /// Asynchronously kills Chrome process.
        /// </summary>
        /// <returns></returns>
        public Task KillAsync() => _currentState.KillAsync(this);

        /// <summary>
        /// Waits for Chrome process exit within a given timeout.
        /// </summary>
        /// <param name="timeout">The maximum wait period.</param>
        /// <returns><c>true</c> if Chrome process has exited within the given <paramref name="timeout"/>,
        /// or <c>false</c> otherwise.</returns>
        public async Task<bool> WaitForExitAsync(TimeSpan? timeout)
        {
            if (timeout.HasValue)
            {
                var taskCompleted = true;
                await _exitCompletionSource.Task.WithTimeout(
                    () =>
                    {
                        taskCompleted = false;
                    }, timeout.Value).ConfigureAwait(false);
                return taskCompleted;
            }

            await _exitCompletionSource.Task.ConfigureAwait(false);
            return true;
        }

        /// <inheritdoc />
        public override string ToString() => $"Chrome process; EndPoint={EndPoint}; State={_currentState}";

        #endregion

        #region Private methods

        private static (List<string> chromeArgs, TempDirectory tempUserDataDir) PrepareChromeArgs(LaunchOptions options)
        {

            var chromeArgs = new List<string>();

            if (!options.IgnoreDefaultArgs)
            {
                chromeArgs.AddRange(GetDefaultArgs(options));
            }
            else if (options.IgnoredDefaultArgs?.Length > 0)
            {
                chromeArgs.AddRange(GetDefaultArgs(options).Except(options.IgnoredDefaultArgs));
            }
            else
            {
                chromeArgs.AddRange(options.Args);
            }

            TempDirectory tempUserDataDir = null;

            if (!chromeArgs.Any(argument => argument.StartsWith("--remote-debugging-", StringComparison.Ordinal)))
            {
                chromeArgs.Add("--remote-debugging-port=0");
            }

            var userDataDirOption = chromeArgs.FirstOrDefault(i => i.StartsWith(UserDataDirArgument, StringComparison.Ordinal));
            if (string.IsNullOrEmpty(userDataDirOption))
            {
                tempUserDataDir = new TempDirectory();
                chromeArgs.Add($"{UserDataDirArgument}={tempUserDataDir.Path.Quote()}");
            }

            return (chromeArgs, tempUserDataDir);
        }

        internal static string[] GetDefaultArgs(LaunchOptions options)
        {
            var chromeArguments = new List<string>(DefaultArgs);

            if (!string.IsNullOrEmpty(options.UserDataDir))
            {
                chromeArguments.Add($"{UserDataDirArgument}={options.UserDataDir.Quote()}");
            }

            if (options.Devtools)
            {
                chromeArguments.Add("--auto-open-devtools-for-tabs");
            }

            if (options.Headless)
            {
                chromeArguments.AddRange(new[] {
                    "--headless",
                    "--hide-scrollbars",
                    "--mute-audio"
                });
            }

            if (options.Args.All(arg => arg.StartsWith("-", StringComparison.Ordinal)))
            {
                chromeArguments.Add("about:blank");
            }

            chromeArguments.AddRange(options.Args);
            return chromeArguments.ToArray();
        }

        #endregion
    }
}
