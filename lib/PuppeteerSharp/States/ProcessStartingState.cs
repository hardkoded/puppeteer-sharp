using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.States
{
    internal class ProcessStartingState(StateManager stateManager) : State(stateManager)
    {
        private const int StderrDrainTimeoutMs = 2000;

        public string LineOutputExpression { get; set; } = "^DevTools listening on (ws:\\/\\/.*)";

        public override Task EnterFromAsync(LauncherBase launcher, State fromState, TimeSpan timeout)
        {
            if (!StateManager.TryEnter(launcher, fromState, this))
            {
                // Delegate StartAsync to current state, because it has already changed since
                // transition to this state was initiated.
                return StateManager.CurrentState.StartAsync(launcher);
            }

            return StartCoreAsync(launcher);
        }

        public override Task StartAsync(LauncherBase p) => p.StartCompletionSource.Task;

        public override Task ExitAsync(LauncherBase launcher, TimeSpan timeout) => StateManager.Exiting.EnterFromAsync(launcher, this, timeout);

        public override Task KillAsync(LauncherBase p) => StateManager.Killing.EnterFromAsync(p, this);

        public override void Dispose(LauncherBase p)
        {
            p.StartCompletionSource.TrySetException(new ObjectDisposedException(p.ToString()));
            base.Dispose(p);
        }

        protected virtual async Task StartCoreAsync(LauncherBase p)
        {
            if (p.ExecutablePath == null || !File.Exists(p.ExecutablePath))
            {
                await p.CleanTempUserDataDirAsync().ConfigureAwait(false);
                throw new ProcessException(
                    $"Browser was not found at the configured executablePath ({p.ExecutablePath})");
            }

            var usePipe = p.Options.Pipe;

            void OnProcessDataReceivedWhileStarting(object sender, DataReceivedEventArgs e)
            {
                if (e.Data != null)
                {
                    var match = Regex.Match(e.Data, LineOutputExpression);
                    if (match.Success)
                    {
                        p.StartCompletionSource.TrySetResult(match.Groups[1].Value);
                    }
                }
            }

            void OnProcessExitedWhileStarting(object sender, EventArgs e)
            {
                // The Exited event can fire before the asynchronous stderr reader has delivered
                // the browser's last lines, so drain it before building the error message.
                // Otherwise the message would be empty and hide why the browser exited.
                Task.Run(() =>
                {
                    var exitCode = "unknown";
                    try
                    {
                        p.Process.WaitForExit(StderrDrainTimeoutMs);
                        exitCode = p.Process.ExitCode.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        // The process might have been disposed already.
                    }

                    p.StartCompletionSource.TrySetException(
                        new ProcessException($"Failed to launch browser! The browser process exited with code {exitCode}. {p.GetRecentLogs()}"));
                });
            }

            void OnProcessExited(object sender, EventArgs e) => StateManager.Exited.EnterFrom(p, StateManager.CurrentState);

            // Always subscribe for DevTools endpoint discovery when not using pipes.
            // Recent stderr lines are captured separately on LauncherBase for both modes.
            if (!usePipe)
            {
                p.Process.ErrorDataReceived += OnProcessDataReceivedWhileStarting;
            }

            p.Process.Exited += OnProcessExitedWhileStarting;
            p.Process.Exited += OnProcessExited;
            CancellationTokenSource cts = null;
            try
            {
                try
                {
                    p.Process.Start();

                    // Always begin reading stderr so ProcessSingleton / Missing X server
                    // diagnostics are available, including when Pipe=true. Start reading
                    // right away so output from a browser that exits quickly is not lost.
                    p.Process.BeginErrorReadLine();

                    // Register after the browser process has started so a host-process
                    // exit can still remove the temporary profile synchronously when
                    // async cleanup cannot run (upstream #15441).
                    p.TempUserDataDir?.RegisterProcessExitCleanup();
                }
                catch (Exception ex)
                {
                    throw new ProcessException($"Failed to launch browser! {ex.Message}", ex);
                }

                await StateManager.Started.EnterFromAsync(p, this).ConfigureAwait(false);

                if (usePipe)
                {
                    // In pipe mode, there's no ws:// URL to wait for.
                    // The pipe transport is the connection mechanism.
                    p.StartCompletionSource.TrySetResult(string.Empty);
                }

                var timeout = p.Options.Timeout;
                if (timeout > 0)
                {
                    cts = new CancellationTokenSource(timeout);
                    cts.Token.Register(() => p.StartCompletionSource.TrySetException(
                        new ProcessException($"Timed out after {timeout} ms while trying to connect to Base!")));
                }

                try
                {
                    await p.StartCompletionSource.Task.ConfigureAwait(false);
                    await StateManager.Started.EnterFromAsync(p, this).ConfigureAwait(false);
                }
                catch
                {
                    await StateManager.Killing.EnterFromAsync(p, this).ConfigureAwait(false);
                    throw;
                }
            }
            finally
            {
                cts?.Dispose();
                p.Process.Exited -= OnProcessExitedWhileStarting;
                if (!usePipe)
                {
                    p.Process.ErrorDataReceived -= OnProcessDataReceivedWhileStarting;
                }
            }
        }
    }
}
