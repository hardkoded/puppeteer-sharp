using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a Base process and any associated temporary user data directory that have created
    /// by Puppeteer and therefore must be cleaned up when no longer needed.
    /// </summary>
    public class LauncherBase : IDisposable
    {
        #region Static fields

        private static int _processCount;

        #endregion

        #region Instance fields

        private readonly LaunchOptions _options;
        private readonly ILogger _logger;
        private readonly TaskCompletionSource<string> _startCompletionSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource<bool> _exitCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private State _currentState = State.Initial;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="LauncherBase"/> instance.
        /// </summary>
        /// <param name="executable">Full path of executable.</param>
        /// <param name="options">Options for launching Base.</param>
        /// <param name="loggerFactory">Logger factory</param>
        public LauncherBase(string executable, LaunchOptions options, ILoggerFactory loggerFactory)
        {
            _options = options;
            _logger = options.LogProcess
                ? loggerFactory.CreateLogger<LauncherBase>()
                : null;

            Process = new Process
            {
                EnableRaisingEvents = true
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

        #endregion

        #region Dispose

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~LauncherBase()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes Base process and any temporary user directory.
        /// </summary>
        /// <param name="disposing">Indicates whether disposal was initiated by <see cref="Dispose()"/> operation.</param>
        protected virtual void Dispose(bool disposing) => _currentState.Dispose(this);

        #endregion

        #region Properties

        /// <summary>
        /// Gets Base process details.
        /// </summary>
        public Process Process { get; }

        /// <summary>
        /// Gets Base endpoint.
        /// </summary>
        public string EndPoint => _startCompletionSource.Task.IsCompleted
            ? _startCompletionSource.Task.Result
            : null;

        /// <summary>
        /// Indicates whether Base process is exiting.
        /// </summary>
        public bool IsExiting => _currentState.IsExiting;

        /// <summary>
        /// Indicates whether Base process has exited.
        /// </summary>
        public bool HasExited => _currentState.IsExited;

        internal TempDirectory TempUserDataDir { get; set; }

        /// <summary>
        /// Gets Base process current state.
        /// </summary>
        protected State CurrentState { get => _currentState; }

        #endregion

        #region Public methods

        /// <summary>
        /// Asynchronously starts Base process.
        /// </summary>
        /// <returns></returns>
        public Task StartAsync() => _currentState.StartAsync(this);

        /// <summary>
        /// Asynchronously waits for graceful Base process exit within a given timeout period.
        /// Kills the Base process if it has not exited within this period.
        /// </summary>
        /// <param name="timeout">The maximum waiting time for a graceful process exit.</param>
        /// <returns></returns>
        public Task EnsureExitAsync(TimeSpan? timeout) => timeout.HasValue
            ? _currentState.ExitAsync(this, timeout.Value)
            : _currentState.KillAsync(this);

        /// <summary>
        /// Asynchronously kills Base process.
        /// </summary>
        /// <returns></returns>
        public Task KillAsync() => _currentState.KillAsync(this);

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
                bool taskCompleted = true;
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

        #endregion

        #region Private methods

        /// <summary>
        /// Set Env Variables
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="customEnv">The customEnv.</param>
        /// <param name="realEnv">The realEnv.</param>
        protected static void SetEnvVariables(IDictionary<string, string> environment, IDictionary<string, string> customEnv, IDictionary realEnv)
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

        #region State machine

        /// <summary>
        /// Represents state machine for Base process instances. The happy path runs along the
        /// following state transitions: <see cref="Initial"/>
        /// -> <see cref="Starting"/>
        /// -> <see cref="Started"/>
        /// -> <see cref="Exiting"/>
        /// -> <see cref="Exited"/>.
        /// -> <see cref="Disposed"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This state machine implements the following state transitions:
        /// <code>
        /// State     Event              Target State Action
        /// ======== =================== ============ ==========================================================
        /// Initial  --StartAsync------> Starting     Start process and wait for endpoint
        /// Initial  --ExitAsync-------> Exited       Cleanup temp user data
        /// Initial  --KillAsync-------> Exited       Cleanup temp user data
        /// Initial  --Dispose---------> Disposed     Cleanup temp user data
        /// Starting --StartAsync------> Starting     -
        /// Starting --ExitAsync-------> Exiting      Wait for process exit
        /// Starting --KillAsync-------> Killing      Kill process
        /// Starting --Dispose---------> Disposed     Kill process; Cleanup temp user data;  throw ObjectDisposedException on outstanding async operations;
        /// Starting --endpoint ready--> Started      Complete StartAsync successfully; Log process start
        /// Starting --process exit----> Exited       Complete StartAsync with exception; Cleanup temp user data
        /// Started  --StartAsync------> Started      -
        /// Started  --EnsureExitAsync-> Exiting      Start exit timer; Log process exit
        /// Started  --KillAsync-------> Killing      Kill process; Log process exit
        /// Started  --Dispose---------> Disposed     Kill process; Log process exit; Cleanup temp user data; throw ObjectDisposedException on outstanding async operations;
        /// Started  --process exit----> Exited       Log process exit; Cleanup temp user data
        /// Exiting  --StartAsync------> Exiting      - (StartAsync throws InvalidOperationException)
        /// Exiting  --ExitAsync-------> Exiting      -
        /// Exiting  --KillAsync-------> Killing      Kill process
        /// Exiting  --Dispose---------> Disposed     Kill process; Cleanup temp user data; throw ObjectDisposedException on outstanding async operations;
        /// Exiting  --exit timeout----> Killing      Kill process
        /// Exiting  --process exit----> Exited       Cleanup temp user data; complete outstanding async operations;
        /// Killing  --StartAsync------> Killing      - (StartAsync throws InvalidOperationException)
        /// Killing  --KillAsync-------> Killing      -
        /// Killing  --Dispose---------> Disposed     Cleanup temp user data; throw ObjectDisposedException on outstanding async operations;
        /// Killing  --process exit----> Exited       Cleanup temp user data; complete outstanding async operations;
        /// Exited   --StartAsync------> Killing      - (StartAsync throws InvalidOperationException)
        /// Exited   --KillAsync-------> Exited       -
        /// Exited   --Dispose---------> Disposed     -
        /// Disposed --StartAsync------> Disposed     -
        /// Disposed --KillAsync-------> Disposed     -
        /// Disposed --Dispose---------> Disposed     -
        /// </code>
        /// </para>
        /// </remarks>
        protected abstract class State
        {
            #region Predefined states

            /// <summary>
            /// Set initial state.
            /// </summary>
            public static readonly State Initial = new InitialState();
            private static readonly StartingState Starting = new StartingState();
            private static readonly StartedState Started = new StartedState();
            private static readonly ExitingState Exiting = new ExitingState();
            private static readonly KillingState Killing = new KillingState();
            private static readonly ExitedState Exited = new ExitedState();
            private static readonly DisposedState Disposed = new DisposedState();

            #endregion

            #region Properties

            /// <summary>
            /// Get If process exists.
            /// </summary>
            public bool IsExiting => this == Killing || this == Exiting;
            /// <summary>
            /// Get If process is exited.
            /// </summary>
            public bool IsExited => this == Exited || this == Disposed;

            #endregion

            #region Methods

            /// <summary>
            /// Attempts thread-safe transitions from a given state to this state.
            /// </summary>
            /// <param name="p">The Base process</param>
            /// <param name="fromState">The state from which state transition takes place</param>
            /// <returns>Returns <c>true</c> if transition is successful, or <c>false</c> if transition
            /// cannot be made because current state does not equal <paramref name="fromState"/>.</returns>
            protected bool TryEnter(LauncherBase p, State fromState)
            {
                if (Interlocked.CompareExchange(ref p._currentState, this, fromState) == fromState)
                {
                    fromState.Leave(p);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Notifies that state machine is about to transition to another state.
            /// </summary>
            /// <param name="p">The Base process</param>
            protected virtual void Leave(LauncherBase p)
            {
            }

            /// <summary>
            /// Handles process start request.
            /// </summary>
            /// <param name="p">The Base process</param>
            /// <returns></returns>
            public virtual Task StartAsync(LauncherBase p) => Task.FromException(InvalidOperation("start"));

            /// <summary>
            /// Handles process exit request.
            /// </summary>
            /// <param name="p">The Base process</param>
            /// <param name="timeout">The maximum waiting time for a graceful process exit.</param>
            /// <returns></returns>
            public virtual Task ExitAsync(LauncherBase p, TimeSpan timeout) => Task.FromException(InvalidOperation("exit"));

            /// <summary>
            /// Handles process kill request.
            /// </summary>
            /// <param name="p">The Base process</param>
            /// <returns></returns>
            public virtual Task KillAsync(LauncherBase p) => Task.FromException(InvalidOperation("kill"));

            /// <summary>
            /// Handles wait for process exit request.
            /// </summary>
            /// <param name="p">The Base process</param>
            /// <returns></returns>
            public virtual Task WaitForExitAsync(LauncherBase p) => p._exitCompletionSource.Task;

            /// <summary>
            /// Handles disposal of process and temporary user directory
            /// </summary>
            /// <param name="p"></param>
            public virtual void Dispose(LauncherBase p) => Disposed.EnterFrom(p, this);

            /// <inheritdoc />
            public override string ToString()
            {
                string name = GetType().Name;
                return name.Substring(0, name.Length - "State".Length);
            }

            private Exception InvalidOperation(string operationName)
                => new InvalidOperationException($"Cannot {operationName} in state {this}");

            /// <summary>
            /// Kills process if it is still alive.
            /// </summary>
            /// <param name="p"></param>
            private static void Kill(LauncherBase p)
            {
                try
                {
                    if (!p.Process.HasExited)
                    {
                        p.Process.Kill();
                    }
                }
                catch (InvalidOperationException)
                {
                    // Ignore
                }
            }

            #endregion

            #region Concrete state classes

            private class InitialState : State
            {
                public override Task StartAsync(LauncherBase p) => Starting.EnterFromAsync(p, this);

                public override Task ExitAsync(LauncherBase p, TimeSpan timeout)
                {
                    Exited.EnterFrom(p, this);
                    return Task.CompletedTask;
                }

                public override Task KillAsync(LauncherBase p)
                {
                    Exited.EnterFrom(p, this);
                    return Task.CompletedTask;
                }

                public override Task WaitForExitAsync(LauncherBase p) => Task.FromException(InvalidOperation("wait for exit"));
            }

            private class StartingState : State
            {
                public Task EnterFromAsync(LauncherBase p, State fromState)
                {
                    if (!TryEnter(p, fromState))
                    {
                        // Delegate StartAsync to current state, because it has already changed since
                        // transition to this state was initiated.
                        return p._currentState.StartAsync(p);
                    }

                    return StartCoreAsync(p);
                }

                public override Task StartAsync(LauncherBase p) => p._startCompletionSource.Task;

                public override Task ExitAsync(LauncherBase p, TimeSpan timeout) => Exiting.EnterFromAsync(p, this, timeout);

                public override Task KillAsync(LauncherBase p) => Killing.EnterFromAsync(p, this);

                public override void Dispose(LauncherBase p)
                {
                    p._startCompletionSource.TrySetException(new ObjectDisposedException(p.ToString()));
                    base.Dispose(p);
                }

                private async Task StartCoreAsync(LauncherBase p)
                {
                    var output = new StringBuilder();

                    void OnProcessDataReceivedWhileStarting(object sender, DataReceivedEventArgs e)
                    {
                        if (e.Data != null)
                        {
                            output.AppendLine(e.Data);
                            var match = Regex.Match(e.Data, "^DevTools listening on (ws:\\/\\/.*)");
                            if (match.Success)
                            {
                                p._startCompletionSource.TrySetResult(match.Groups[1].Value);
                            }
                        }
                    }

                    void OnProcessExitedWhileStarting(object sender, EventArgs e)
                        => p._startCompletionSource.TrySetException(new ProcessException($"Failed to launch Base! {output}"));
                    void OnProcessExited(object sender, EventArgs e) => Exited.EnterFrom(p, p._currentState);

                    p.Process.ErrorDataReceived += OnProcessDataReceivedWhileStarting;
                    p.Process.Exited += OnProcessExitedWhileStarting;
                    p.Process.Exited += OnProcessExited;
                    CancellationTokenSource cts = null;
                    try
                    {
                        p.Process.Start();
                        await Started.EnterFromAsync(p, this).ConfigureAwait(false);

                        p.Process.BeginErrorReadLine();

                        int timeout = p._options.Timeout;
                        if (timeout > 0)
                        {
                            cts = new CancellationTokenSource(timeout);
                            cts.Token.Register(() => p._startCompletionSource.TrySetException(
                                new ProcessException($"Timed out after {timeout} ms while trying to connect to Base!")));
                        }

                        try
                        {
                            await p._startCompletionSource.Task.ConfigureAwait(false);
                            await Started.EnterFromAsync(p, this).ConfigureAwait(false);
                        }
                        catch
                        {
                            await Killing.EnterFromAsync(p, this).ConfigureAwait(false);
                            throw;
                        }
                    }
                    finally
                    {
                        cts?.Dispose();
                        p.Process.Exited -= OnProcessExitedWhileStarting;
                        p.Process.ErrorDataReceived -= OnProcessDataReceivedWhileStarting;
                    }
                }
            }

            private class StartedState : State
            {
                public Task EnterFromAsync(LauncherBase p, State fromState)
                {
                    if (TryEnter(p, fromState))
                    {
                        // Process has not exited or been killed since transition to this state was initiated
                        LogProcessCount(p, Interlocked.Increment(ref _processCount));
                    }

                    return Task.CompletedTask;
                }

                protected override void Leave(LauncherBase p)
                    => LogProcessCount(p, Interlocked.Decrement(ref _processCount));

                public override Task StartAsync(LauncherBase p) => Task.CompletedTask;

                public override Task ExitAsync(LauncherBase p, TimeSpan timeout) => Exiting.EnterFromAsync(p, this, timeout);

                public override Task KillAsync(LauncherBase p) => Killing.EnterFromAsync(p, this);

                private static void LogProcessCount(LauncherBase p, int processCount)
                {
                    try
                    {
                        p._logger?.LogInformation("Process Count: {ProcessCount}", processCount);
                    }
                    catch
                    {
                        // Prevent logging exception from causing havoc
                    }
                }
            }

            private class ExitingState : State
            {
                public Task EnterFromAsync(LauncherBase p, State fromState, TimeSpan timeout)
                    => !TryEnter(p, fromState) ? p._currentState.ExitAsync(p, timeout) : ExitAsync(p, timeout);

                public override async Task ExitAsync(LauncherBase p, TimeSpan timeout)
                {
                    var waitForExitTask = WaitForExitAsync(p);
                    await waitForExitTask.WithTimeout(
                        async () =>
                        {
                            await Killing.EnterFromAsync(p, this).ConfigureAwait(false);
                            await waitForExitTask.ConfigureAwait(false);
                        },
                        timeout,
                        CancellationToken.None).ConfigureAwait(false);
                }

                public override Task KillAsync(LauncherBase p) => Killing.EnterFromAsync(p, this);
            }

            private class KillingState : State
            {
                public async Task EnterFromAsync(LauncherBase p, State fromState)
                {
                    if (!TryEnter(p, fromState))
                    {
                        // Delegate KillAsync to current state, because it has already changed since
                        // transition to this state was initiated.
                        await p._currentState.KillAsync(p).ConfigureAwait(false);
                    }

                    try
                    {
                        if (!p.Process.HasExited)
                        {
                            p.Process.Kill();
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // Ignore
                        return;
                    }

                    await WaitForExitAsync(p).ConfigureAwait(false);
                }

                public override Task ExitAsync(LauncherBase p, TimeSpan timeout) => WaitForExitAsync(p);

                public override Task KillAsync(LauncherBase p) => WaitForExitAsync(p);
            }

            private class ExitedState : State
            {
                public void EnterFrom(LauncherBase p, State fromState)
                {
                    while (!TryEnter(p, fromState))
                    {
                        // Current state has changed since transition to this state was requested.
                        // Therefore retry transition to this state from the current state. This ensures
                        // that Leave() operation of current state is properly called.
                        fromState = p._currentState;
                        if (fromState == this)
                        {
                            return;
                        }
                    }

                    p._exitCompletionSource.TrySetResult(true);
                    p.TempUserDataDir?.Dispose();
                }

                public override Task ExitAsync(LauncherBase p, TimeSpan timeout) => Task.CompletedTask;

                public override Task KillAsync(LauncherBase p) => Task.CompletedTask;

                public override Task WaitForExitAsync(LauncherBase p) => Task.CompletedTask;
            }

            private class DisposedState : State
            {
                public void EnterFrom(LauncherBase p, State fromState)
                {
                    if (!TryEnter(p, fromState))
                    {
                        // Delegate Dispose to current state, because it has already changed since
                        // transition to this state was initiated.
                        p._currentState.Dispose(p);
                    }
                    else if (fromState != Exited)
                    {
                        Kill(p);

                        p._exitCompletionSource.TrySetException(new ObjectDisposedException(p.ToString()));
                        p.TempUserDataDir?.Dispose();
                    }
                }

                public override Task StartAsync(LauncherBase p) => throw new ObjectDisposedException(p.ToString());

                public override Task ExitAsync(LauncherBase p, TimeSpan timeout) => throw new ObjectDisposedException(p.ToString());

                public override Task KillAsync(LauncherBase p) => throw new ObjectDisposedException(p.ToString());

                public override void Dispose(LauncherBase p)
                {
                    // Nothing to do
                }
            }

            #endregion
        }

        #endregion
    }
}
