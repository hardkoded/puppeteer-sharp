using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.States;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a Base process and any associated temporary user data directory that have created
    /// by Puppeteer and therefore must be cleaned up when no longer needed.
    /// </summary>
    public abstract class LauncherBase : IDisposable
    {
        private const int MaxRecentLogLines = 100;
        private readonly ConcurrentQueue<string> _recentLogs = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="LauncherBase"/> class.
        /// </summary>
        /// <param name="executable">Full path of executable.</param>
        /// <param name="options">Options for launching Base.</param>
        protected LauncherBase(string executable, LaunchOptions options)
        {
            StateManager = new StateManager();
            StateManager.Starting = new ProcessStartingState(StateManager);

            Options = options ?? throw new ArgumentNullException(nameof(options));

            Process = new Process
            {
                EnableRaisingEvents = true,
            };
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.CreateNoWindow = true;
            Process.StartInfo.FileName = executable;
            Process.StartInfo.RedirectStandardError = true;
            ExecutablePath = executable;

            SetEnvVariables(Process.StartInfo.Environment, options.Env, Environment.GetEnvironmentVariables());

            // Always capture stderr so launch diagnostics (ProcessSingleton, Missing X
            // server, etc.) are available even when Pipe=true, matching upstream
            // browserProcess.getRecentLogs().
            Process.ErrorDataReceived += OnErrorDataReceived;
            if (options.DumpIO)
            {
                Process.ErrorDataReceived += (_, e) => Console.Error.WriteLine(e.Data);
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="LauncherBase"/> class.
        /// </summary>
        ~LauncherBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets Base process details.
        /// </summary>
        public Process Process { get; }

        /// <summary>
        /// Gets Base endpoint.
        /// </summary>
        public string EndPoint => StartCompletionSource.Task.IsCompleted
            ? StartCompletionSource.Task.Result
            : null;

        /// <summary>
        /// Indicates whether Base process is exiting.
        /// </summary>
        public bool IsExiting => StateManager.CurrentState.IsExiting;

        /// <summary>
        /// Indicates whether Base process has exited.
        /// </summary>
        public bool HasExited => StateManager.CurrentState.IsExited;

        /// <summary>
        /// Gets the browser executable path used to launch the process.
        /// </summary>
        internal string ExecutablePath { get; }

        internal StateManager StateManager { get; }

        internal TaskCompletionSource<bool> ExitCompletionSource { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal TaskCompletionSource<string> StartCompletionSource { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal LaunchOptions Options { get; }

        internal TempDirectory TempUserDataDir { get; init; }

        /// <summary>
        /// Gets the pipe transport when pipe mode is enabled.
        /// </summary>
        internal virtual PipeTransport PipeTransport => null;

        /// <summary>
        /// Gets Base process current state.
        /// </summary>
        internal State CurrentState => StateManager.CurrentState;

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Asynchronously starts Base process.
        /// </summary>
        /// <returns>Task which resolves when after start process begins.</returns>
        public Task StartAsync() => StateManager.CurrentState.StartAsync(this);

        /// <summary>
        /// Asynchronously waits for graceful Base process exit within a given timeout period.
        /// Kills the Base process if it has not exited within this period.
        /// </summary>
        /// <param name="timeout">The maximum waiting time for a graceful process exit.</param>
        /// <returns>Task which resolves when the process is exited or killed.</returns>
        public Task EnsureExitAsync(TimeSpan? timeout) => timeout.HasValue
            ? StateManager.CurrentState.ExitAsync(this, timeout.Value)
            : StateManager.CurrentState.KillAsync(this);

        /// <summary>
        /// Asynchronously kills Base process.
        /// </summary>
        /// <returns>Task which resolves when the process is killed.</returns>
        public Task KillAsync() => StateManager.CurrentState.KillAsync(this);

        /// <summary>
        /// Waits for Base process exit within a given timeout.
        /// </summary>
        /// <param name="timeout">The maximum wait period.</param>
        /// <returns><c>true</c> if Base process has exited within the given <paramref name="timeout"/>,
        /// or <c>false</c> otherwise.</returns>
        public async Task<bool> WaitForExitAsync(TimeSpan? timeout)
        {
            if (timeout.HasValue)
            {
                var taskCompleted = true;
                await ExitCompletionSource.Task.WithTimeout(
                    () =>
                    {
                        taskCompleted = false;
                    },
                    timeout.Value).ConfigureAwait(false);
                return taskCompleted;
            }

            await ExitCompletionSource.Task.ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Deletes the temporary user data directory if one was created for this launch.
        /// Cleanup errors are swallowed so they cannot become uncaught exceptions.
        /// </summary>
        /// <returns>A task that completes when cleanup finishes.</returns>
        internal Task CleanTempUserDataDirAsync()
            => TempUserDataDir is { } tempUserDataDir
                ? tempUserDataDir.DeleteAsync()
                : Task.CompletedTask;

        /// <summary>
        /// Recent stderr lines from the browser process, used for launch diagnostics.
        /// </summary>
        /// <returns>A snapshot of the most recent stderr lines.</returns>
        internal string GetRecentLogs() => string.Join("\n", _recentLogs);

        /// <summary>
        /// Cleans up temporary user data directory.
        /// </summary>
        internal virtual void OnExit()
        {
            if (TempUserDataDir is { } tempUserDataDir)
            {
                tempUserDataDir
                    .DeleteAsync()
                    .ContinueWith(
                        t => ExitCompletionSource.TrySetResult(true),
                        TaskScheduler.Default);
            }
            else
            {
                ExitCompletionSource.TrySetResult(true);
            }
        }

        /// <summary>
        /// Set Env Variables.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="customEnv">The customEnv.</param>
        /// <param name="realEnv">The realEnv.</param>
        protected static void SetEnvVariables(IDictionary<string, string> environment, IDictionary<string, string> customEnv, IDictionary realEnv)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            if (realEnv == null)
            {
                throw new ArgumentNullException(nameof(realEnv));
            }

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

        /// <summary>
        /// Disposes Base process and any temporary user directory.
        /// </summary>
        /// <param name="disposing">Indicates whether disposal was initiated by <see cref="Dispose()"/> operation.</param>
        protected virtual void Dispose(bool disposing) => StateManager.CurrentState.Dispose(this);

        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }

            _recentLogs.Enqueue(e.Data);
            while (_recentLogs.Count > MaxRecentLogLines && _recentLogs.TryDequeue(out _))
            {
            }
        }
    }
}
