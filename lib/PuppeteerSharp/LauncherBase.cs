using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.States;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a Base process and any associated temporary user data directory that have created
    /// by Puppeteer and therefore must be cleaned up when no longer needed.
    /// </summary>
    public abstract class LauncherBase : IDisposable
    {
        private readonly StateManager _stateManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LauncherBase"/> class.
        /// </summary>
        /// <param name="executable">Full path of executable.</param>
        /// <param name="options">Options for launching Base.</param>
        public LauncherBase(string executable, LaunchOptions options)
        {
            _stateManager = new StateManager();
            _stateManager.Starting = new ProcessStartingState(_stateManager);

            Options = options ?? throw new ArgumentNullException(nameof(options));

            Process = new Process
            {
                EnableRaisingEvents = true,
            };
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.FileName = executable;
            Process.StartInfo.RedirectStandardError = true;

            SetEnvVariables(Process.StartInfo.Environment, options.Env, Environment.GetEnvironmentVariables());

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
        public bool IsExiting => _stateManager.CurrentState.IsExiting;

        /// <summary>
        /// Indicates whether Base process has exited.
        /// </summary>
        public bool HasExited => _stateManager.CurrentState.IsExited;

        internal TaskCompletionSource<bool> ExitCompletionSource { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal TaskCompletionSource<string> StartCompletionSource { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal LaunchOptions Options { get; }

        internal TempDirectory TempUserDataDir { get; set; }

        /// <summary>
        /// Gets Base process current state.
        /// </summary>
        internal State CurrentState => _stateManager.CurrentState;

        /// <summary>
        /// Default build.
        /// </summary>
        /// <returns>A tasks that resolves when the build is obtained.</returns>
        public abstract Task<string> GetDefaultBuildIdAsync();

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
        public Task StartAsync() => _stateManager.CurrentState.StartAsync(this);

        /// <summary>
        /// Asynchronously waits for graceful Base process exit within a given timeout period.
        /// Kills the Base process if it has not exited within this period.
        /// </summary>
        /// <param name="timeout">The maximum waiting time for a graceful process exit.</param>
        /// <returns>Task which resolves when the process is exited or killed.</returns>
        public Task EnsureExitAsync(TimeSpan? timeout) => timeout.HasValue
            ? _stateManager.CurrentState.ExitAsync(this, timeout.Value)
            : _stateManager.CurrentState.KillAsync(this);

        /// <summary>
        /// Asynchronously kills Base process.
        /// </summary>
        /// <returns>Task which resolves when the process is killed.</returns>
        public Task KillAsync() => _stateManager.CurrentState.KillAsync(this);

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
        protected virtual void Dispose(bool disposing) => _stateManager.CurrentState.Dispose(this);
    }
}
