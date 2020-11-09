using System;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.States
{
    internal class ExitingState : State
    {
        public ExitingState(StateManager stateManager) : base(stateManager)
        {
        }

        public override Task EnterFromAsync(LauncherBase p, State fromState, TimeSpan timeout)
            => !StateManager.TryEnter(p, fromState, this) ? StateManager.CurrentState.ExitAsync(p, timeout) : ExitAsync(p, timeout);

        public override async Task ExitAsync(LauncherBase p, TimeSpan timeout)
        {
            var waitForExitTask = WaitForExitAsync(p);
            await waitForExitTask.WithTimeout(
                async () =>
                {
                    await StateManager.Killing.EnterFromAsync(p, this, timeout).ConfigureAwait(false);
                    await waitForExitTask.ConfigureAwait(false);
                },
                timeout,
                CancellationToken.None).ConfigureAwait(false);
        }

        public override Task KillAsync(LauncherBase p) => StateManager.Killing.EnterFromAsync(p, this);
    }
}
