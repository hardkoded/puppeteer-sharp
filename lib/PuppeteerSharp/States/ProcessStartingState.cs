using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.States
{
    internal class ProcessStartingState : State
    {
        public ProcessStartingState(StateManager stateManager) : base(stateManager)
        {
        }

        public override Task EnterFromAsync(LauncherBase p, State fromState, TimeSpan timeout)
        {
            if (!StateManager.TryEnter(p, fromState, this))
            {
                // Delegate StartAsync to current state, because it has already changed since
                // transition to this state was initiated.
                return StateManager.CurrentState.StartAsync(p);
            }

            return StartCoreAsync(p);
        }

        public override Task StartAsync(LauncherBase p) => p.StartCompletionSource.Task;

        public override Task ExitAsync(LauncherBase p, TimeSpan timeout) => StateManager.Exiting.EnterFromAsync(p, this, timeout);

        public override Task KillAsync(LauncherBase p) => StateManager.Killing.EnterFromAsync(p, this);

        public override void Dispose(LauncherBase p)
        {
            p.StartCompletionSource.TrySetException(new ObjectDisposedException(p.ToString()));
            base.Dispose(p);
        }

        protected virtual async Task StartCoreAsync(LauncherBase p)
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
                        p.StartCompletionSource.TrySetResult(match.Groups[1].Value);
                    }
                }
            }

            void OnProcessExitedWhileStarting(object sender, EventArgs e)
                => p.StartCompletionSource.TrySetException(new ProcessException($"Failed to launch browser! {output}"));

            void OnProcessExited(object sender, EventArgs e) => StateManager.Exited.EnterFrom(p, StateManager.CurrentState);

            p.Process.ErrorDataReceived += OnProcessDataReceivedWhileStarting;
            p.Process.Exited += OnProcessExitedWhileStarting;
            p.Process.Exited += OnProcessExited;
            CancellationTokenSource cts = null;
            try
            {
                p.Process.Start();
                await StateManager.Started.EnterFromAsync(p, this).ConfigureAwait(false);

                p.Process.BeginErrorReadLine();

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
                p.Process.ErrorDataReceived -= OnProcessDataReceivedWhileStarting;
            }
        }
    }
}
