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
    /// Represents a Firefox process and any associated temporary user data directory that have created
    /// by Puppeteer and therefore must be cleaned up when no longer needed.
    /// </summary>
    public class FirefoxProcess : ChromiumProcess
    {
        #region Instance fields

        private readonly TempDirectory _tempUserDataDir;
        private readonly TaskCompletionSource<string> _startCompletionSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource<bool> _exitCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private State _currentState = State.Initial;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="FirefoxProcess"/> instance.
        /// </summary>
        /// <param name="firefoxExecutable">Full path of Firefox executable.</param>
        /// <param name="options">Options for launching Firefox.</param>
        /// <param name="loggerFactory">Logger factory</param>
        public FirefoxProcess(string firefoxExecutable, LaunchOptions options, ILoggerFactory loggerFactory)
        {
            _options = options;
            _logger = options.LogProcess
                ? loggerFactory.CreateLogger<FirefoxProcess>()
                : null;

            List<string> firefoxArgs;
            (firefoxArgs, _tempUserDataDir) = PrepareFirefoxArgs(options);

            Process = new Process
            {
                EnableRaisingEvents = true
            };
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.FileName = firefoxExecutable;
            Process.StartInfo.Arguments = string.Join(" ", firefoxArgs);
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
        ~FirefoxProcess()
        {
            Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets Firefox process details.
        /// </summary>
        public Process Process { get; }

        /// <summary>
        /// Gets Firefox endpoint.
        /// </summary>
        public string EndPoint => _startCompletionSource.Task.IsCompleted
            ? _startCompletionSource.Task.Result
            : null;

        /// <summary>
        /// Indicates whether Firefox process is exiting.
        /// </summary>
        public bool IsExiting => _currentState.IsExiting;

        /// <summary>
        /// Indicates whether Firefox process has exited.
        /// </summary>
        public bool HasExited => _currentState.IsExited;

        #endregion

        #region Public methods

        /// <summary>
        /// Asynchronously starts Firefox process.
        /// </summary>
        /// <returns></returns>
        public Task StartAsync() => _currentState.StartAsync(this);

        /// <summary>
        /// Asynchronously waits for graceful Firefox process exit within a given timeout period.
        /// Kills the Firefox process if it has not exited within this period.
        /// </summary>
        /// <param name="timeout">The maximum waiting time for a graceful process exit.</param>
        /// <returns></returns>
        public Task EnsureExitAsync(TimeSpan? timeout) => timeout.HasValue
            ? _currentState.ExitAsync(this, timeout.Value)
            : _currentState.KillAsync(this);

        /// <summary>
        /// Asynchronously kills Firefox process.
        /// </summary>
        /// <returns></returns>
        public Task KillAsync() => _currentState.KillAsync(this);

        /// <summary>
        /// Waits for Firefox process exit within a given timeout.
        /// </summary>
        /// <param name="timeout">The maximum wait period.</param>
        /// <returns><c>true</c> if Firefox process has exited within the given <paramref name="timeout"/>,
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
        public override string ToString() => $"Firefox process; EndPoint={EndPoint}; State={_currentState}";

        #endregion

        #region Private methods

        private static (List<string> firefoxArgs, TempDirectory tempUserDataDir) PrepareFirefoxArgs(LaunchOptions options)
        {
            var firefoxArgs = new List<string>();

            if (!options.IgnoreDefaultArgs)
            {
                firefoxArgs.AddRange(GetDefaultArgs(options));
            }
            else if (options.IgnoredDefaultArgs?.Length > 0)
            {
                firefoxArgs.AddRange(GetDefaultArgs(options).Except(options.IgnoredDefaultArgs));
            }
            else
            {
                firefoxArgs.AddRange(options.Args);
            }

            TempDirectory tempUserDataDir = null;

            if (!firefoxArgs.Any(argument => argument.StartsWith("--remote-debugging-", StringComparison.Ordinal)))
            {
                firefoxArgs.Add("--remote-debugging-port=0");
            }

            var userDataDirOption = firefoxArgs.FirstOrDefault(i => i.StartsWith(UserDataDirArgument, StringComparison.Ordinal));
            if (string.IsNullOrEmpty(userDataDirOption))
            {
                tempUserDataDir = new TempDirectory();
                firefoxArgs.Add($"{UserDataDirArgument}={tempUserDataDir.Path.Quote()}");
            }

            return (firefoxArgs, tempUserDataDir);
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

        private static void SetEnvVariables(IDictionary<string, string> environment, IDictionary<string, string> customEnv, IDictionary realEnv)
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
