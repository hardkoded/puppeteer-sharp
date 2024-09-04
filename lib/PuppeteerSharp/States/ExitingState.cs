using System;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.States
{
    internal class ExitingState(StateManager stateManager) : State(stateManager)
    {
        public override Task EnterFromAsync(LauncherBase launcher, State fromState, TimeSpan timeout)
            => !StateManager.TryEnter(launcher, fromState, this)
                ? StateManager.CurrentState.ExitAsync(launcher, timeout)
                : ExitAsync(launcher, timeout);

        public override async Task ExitAsync(LauncherBase launcher, TimeSpan timeout)
        {
            var waitForExitTask = WaitForExitAsync(launcher);
            await waitForExitTask.WithTimeout(
                async () =>
                {
                    await StateManager.Killing.EnterFromAsync(launcher, this, timeout).ConfigureAwait(false);
                    await waitForExitTask.ConfigureAwait(false);
                },
                timeout,
                CancellationToken.None).ConfigureAwait(false);
        }

        public override Task KillAsync(LauncherBase p) => StateManager.Killing.EnterFromAsync(p, this);
    }
}
