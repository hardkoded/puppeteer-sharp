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
    /// Represents a Chrome process and any associated temporary user data directory that have created
    /// by Puppeteer and therefore must be cleaned up when no longer needed.
    /// </summary>
    public class ChromeProcess : IDisposable
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

        #region Static fields

        private static int _processCount;

        #endregion

        #region Instance fields

        private readonly LaunchOptions _options;
        private readonly TempDirectory _tempUserDataDir;
        private readonly ILogger _logger;
        private readonly TaskCompletionSource<bool> _exitCompletionSource = new TaskCompletionSource<bool>();
        private State _currentState = State.Initial;
        private Task<string> _startTask;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="ChromeProcess"/> instance.
        /// </summary>
        /// <param name="chromeExecutable">Full path of chrome executable.</param>
        /// <param name="options">Options for launching chrome.</param>
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

        /// <inheritdoc />
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Initiates asynchronous disposal of chrome process and any temporary user directory.
        /// </summary>
        /// <param name="disposing">Indicates whether disposal was initiated by <see cref="Dispose()"/> operation.</param>
        protected virtual void Dispose(bool disposing) => _ = KillAsync();

        #endregion

        #region Properties

        /// <summary>
        /// Gets chrome process details.
        /// </summary>
        public Process Process { get; }

        /// <summary>
        /// Gets chrome endpoint.
        /// </summary>
        public string EndPoint => _startTask?.Result;

        /// <summary>
        /// Indicates whether chrome process is exiting. 
        /// </summary>
        public bool IsClosing => _currentState.IsExiting;

        /// <summary>
        /// Indicates whether chrome process has exited. 
        /// </summary>
        public bool IsClosed => _currentState.IsExited;

        #endregion

        #region Public methods

        /// <summary>
        /// Asynchronously starts chrome process.
        /// </summary>
        /// <returns></returns>
        public Task StartAsync() => _currentState.StartAsync(this);

        /// <summary>
        /// Asynchronously kills chrome process.
        /// </summary>
        /// <returns></returns>
        public Task KillAsync() => _currentState.KillAsync(this);

        /// <summary>
        /// Waits for chrome process exit.
        /// </summary>
        /// <returns></returns>
        public Task WaitForExitAsync() => _currentState.WaitForExitAsync(this);

        #endregion

        #region Private methods

        private static (List<string> chromeArgs, TempDirectory tempUserDataDir) PrepareChromeArgs(LaunchOptions options)
        {
            var chromeArgs = new List<string>(DefaultArgs);
            TempDirectory tempUserDataDir = null;

            if (options.AppMode)
            {
                options.Headless = false;
            }
            else
            {
                chromeArgs.AddRange(AutomationArgs);
            }

            if (!options.IgnoreDefaultArgs ||
                !chromeArgs.Any(argument => argument.StartsWith("--remote-debugging-", StringComparison.Ordinal)))
            {
                chromeArgs.Add("--remote-debugging-port=0");
            }

            var userDataDirOption = options.Args.FirstOrDefault(i => i.StartsWith(UserDataDirArgument, StringComparison.Ordinal));
            if (string.IsNullOrEmpty(userDataDirOption))
            {
                if (string.IsNullOrEmpty(options.UserDataDir))
                {
                    tempUserDataDir = new TempDirectory();
                    chromeArgs.Add($"{UserDataDirArgument}={tempUserDataDir.Path.Quote()}");
                }
                else
                {
                    chromeArgs.Add($"{UserDataDirArgument}={options.UserDataDir.Quote()}");
                }
            }
            else
            {
                options.UserDataDir = userDataDirOption.Replace($"{UserDataDirArgument}=", string.Empty).UnQuote();
            }

            if (options.Devtools)
            {
                chromeArgs.Add("--auto-open-devtools-for-tabs");
                options.Headless = false;
            }

            if (options.Headless)
            {
                chromeArgs.AddRange(new[]{
                    "--headless",
                    "--disable-gpu",
                    "--hide-scrollbars",
                    "--mute-audio"
                });
            }

            if (!options.IgnoreDefaultArgs && options.Args.Any() && options.Args.All(arg => arg.StartsWith("-")))
            {
                chromeArgs.Add("about:blank");
            }

            if (options.Args.Any())
            {
                chromeArgs.AddRange(options.Args);
            }

            return (chromeArgs, tempUserDataDir);
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

        #region State machine

        /// <summary>
        /// Represents state machine for chrome process instances. The happy path runs along the
        /// following state transitions: <see cref="Initial"/>
        /// -> <see cref="Starting"/>
        /// -> <see cref="Started"/>
        /// -> <see cref="Killing"/>
        /// -> <see cref="Exited"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This state machine implements the following state transitions:
        /// <code>
        /// State     Event              Target State Action
        /// ======== =================== ============ ==========================================================
        /// Initial  --StartAsync------> Starting     Start process and wait for endpoint
        /// Initial  --KillAsync-------> Exited       Cleanup temp user data
        /// Starting --StartAsync------> Starting     -
        /// Starting --KillAsync-------> Killing      -
        /// Starting --endpoint ready--> Started      Complete StartAsync successfully; Log process start
        /// Starting --process exit----> Exited       Complete StartAsync with exception; Cleanup temp user data
        /// Started  --StartAsync------> Started      -
        /// Started  --KillAsync-------> Killing      Kill process; Log process exit
        /// Started  --process exit----> Exited       Log process exit; Cleanup temp user data
        /// Killing  --StartAsync------> Killing      - (StartAsync throws InvalidOperationException)
        /// Killing  --KillAsync-------> Killing      -
        /// Killing  --process exit----> Exited       Cleanup temp user data
        /// Exited   --StartAsync------> Killing      - (StartAsync throws InvalidOperationException)
        /// Exited   --KillAsync-------> Exited       -
        /// </code>
        /// </para>
        /// <para>
        /// Each state transition is initiated by invocation of <see cref="EnterFromAsync"/> on the target state.
        /// </para>
        /// </remarks>
        private abstract class State
        {
            #region Predefined states

            public static readonly State Initial = new InitialState();
            public static readonly State Starting = new StartingState();
            public static readonly State Started = new StartedState();
            public static readonly State Killing = new KillingState();
            public static readonly State Exited = new ExitedState();

            #endregion

            #region Properties

            public bool IsExiting => this == Killing;
            public bool IsExited => this == Exited;

            #endregion

            #region Methods

            /// <summary>
            /// Transitions current state of chrome process to this state.
            /// </summary>
            /// <param name="p">The chrome process</param>
            /// <param name="fromState">The state from which state transition takes place</param>
            /// <returns></returns>
            protected virtual Task EnterFromAsync(ChromeProcess p, State fromState)
            {
                TryEnter(p, fromState);
                return Task.CompletedTask;
            }

            /// <summary>
            /// Attempts thread-safe transitions from a given state to this state.
            /// </summary>
            /// <param name="p">The chrome process</param>
            /// <param name="fromState">The state from which state transition takes place</param>
            /// <returns>Returns <c>true</c> if transition is successful, or <c>false</c> if transition
            /// cannot be made because current state does not equal <paramref name="fromState"/>.</returns>
            protected bool TryEnter(ChromeProcess p, State fromState)
            {
                if (Interlocked.CompareExchange(ref p._currentState, this, fromState) == fromState)
                {
                    fromState.Leave(p);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Notifies that chrome process is about to transition to another state.
            /// </summary>
            /// <param name="p">The chrome process</param>
            protected virtual void Leave(ChromeProcess p)
            { }

            /// <summary>
            /// Handles process start request.
            /// </summary>
            /// <param name="p">The chrome process</param>
            /// <returns></returns>
            public virtual Task StartAsync(ChromeProcess p) => Task.FromException(InvalidOperation("start"));

            /// <summary>
            /// Handles process kill request.
            /// </summary>
            /// <param name="p">The chrome process</param>
            /// <returns></returns>
            public virtual Task KillAsync(ChromeProcess p) => Task.FromException(InvalidOperation("kill"));

            /// <summary>
            /// Handles wait for process exit request.
            /// </summary>
            /// <param name="p">The chrome process</param>
            /// <returns></returns>
            public virtual Task WaitForExitAsync(ChromeProcess p) => p._exitCompletionSource.Task;

            private Exception InvalidOperation(string operationName)
                => new InvalidOperationException($"Cannot {operationName} in state {this}");

            public override string ToString()
            {
                var name = GetType().Name;
                return name.Substring(0, name.Length - "State".Length);
            }

            #endregion

            #region Concrete state classes

            private class InitialState : State
            {
                public override Task StartAsync(ChromeProcess p) => Starting.EnterFromAsync(p, this);

                public override Task KillAsync(ChromeProcess p) => Exited.EnterFromAsync(p, this);

                public override Task WaitForExitAsync(ChromeProcess p) => Task.FromException(InvalidOperation("wait for exit"));
            }

            private class StartingState : State
            {
                protected override Task EnterFromAsync(ChromeProcess p, State fromState)
                {
                    if (!TryEnter(p, fromState))
                    {
                        // Delegate StartAsync to current state, because it has already changed since
                        // transition to this state was initiated.
                        return p._currentState.StartAsync(p);
                    }

                    p._startTask = StartCoreAsync(p);
                    return p._startTask;
                }

                public override Task StartAsync(ChromeProcess p) => p._startTask;

                public override Task KillAsync(ChromeProcess p) => Killing.EnterFromAsync(p, this);

                private async Task<string> StartCoreAsync(ChromeProcess p)
                {
                    var startCompletionSource = new TaskCompletionSource<string>();
                    var output = new StringBuilder();

                    void OnChromeDataReceivedWhileStarting(object sender, DataReceivedEventArgs e)
                    {
                        if (e.Data != null)
                        {
                            output.AppendLine(e.Data);
                            var match = Regex.Match(e.Data, "^DevTools listening on (ws:\\/\\/.*)");
                            if (match.Success)
                            {
                                startCompletionSource.SetResult(match.Groups[1].Value);
                            }
                        }
                    }
                    void OnChromeExitedWhileStarting(object sender, EventArgs e)
                    {
                        startCompletionSource.SetException(new ChromeProcessException($"Failed to launch chrome! {output}"));
                    }
                    void OnChromeExited(object sender, EventArgs e)
                    {
                        _ = Exited.EnterFromAsync(p, p._currentState);
                    }

                    p.Process.ErrorDataReceived += OnChromeDataReceivedWhileStarting;
                    p.Process.Exited += OnChromeExitedWhileStarting;
                    p.Process.Exited += OnChromeExited;
                    CancellationTokenSource cts = null;
                    try
                    {
                        p.Process.Start();
                        await Started.EnterFromAsync(p, this).ConfigureAwait(false);

                        p.Process.BeginErrorReadLine();

                        var timeout = p._options.Timeout;
                        if (timeout > 0)
                        {
                            cts = new CancellationTokenSource(timeout);
                            cts.Token.Register(() => startCompletionSource.TrySetException(
                                new ChromeProcessException($"Timed out after {timeout} ms while trying to connect to Chrome!")));
                        }

                        try
                        {
                            var endPoint = await startCompletionSource.Task.ConfigureAwait(false);
                            await Started.EnterFromAsync(p, this);
                            return endPoint;
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
                        p.Process.Exited -= OnChromeExitedWhileStarting;
                        p.Process.ErrorDataReceived -= OnChromeDataReceivedWhileStarting;
                    }
                }
            }

            private class StartedState : State
            {
                protected override Task EnterFromAsync(ChromeProcess p, State fromState)
                {
                    if (TryEnter(p, fromState))
                    {
                        // Process has not exited or been killed since transition to this state was initiated
                        p._logger?.LogInformation("Process Count: {ProcessCount}", Interlocked.Increment(ref _processCount));
                    }
                    return Task.CompletedTask;
                }

                protected override void Leave(ChromeProcess p)
                    => p._logger?.LogInformation("Process Count: {ProcessCount}", Interlocked.Decrement(ref _processCount));

                public override Task StartAsync(ChromeProcess p) => Task.CompletedTask;

                public override Task KillAsync(ChromeProcess p) => Killing.EnterFromAsync(p, this);
            }

            private class KillingState : State
            {
                protected override Task EnterFromAsync(ChromeProcess p, State fromState)
                {
                    if (!TryEnter(p, fromState))
                    {
                        // Delegate KillAsync to current state, because it has already changed since
                        // transition to this state was initiated.
                        return p._currentState.KillAsync(p);
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
                    }

                    return WaitForExitAsync(p);
                }

                public override Task KillAsync(ChromeProcess p) => WaitForExitAsync(p);
            }

            private class ExitedState : State
            {
                protected override Task EnterFromAsync(ChromeProcess p, State fromState)
                {
                    while (!TryEnter(p, fromState))
                    {
                        // Current state has changed since transition to this state was requested.
                        // Therefore retry transition to this state from the current state. This ensures
                        // that Leave() operation of current state is properly called.
                        fromState = p._currentState;
                    }

                    p._exitCompletionSource.SetResult(true);
                    p._tempUserDataDir?.Dispose();
                    return Task.CompletedTask;
                }

                public override Task KillAsync(ChromeProcess p) => Task.CompletedTask;

                public override Task WaitForExitAsync(ChromeProcess p) => Task.CompletedTask;
            }

            #endregion
        }

        #endregion
    }
}