using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.States
{
    internal class KillingState(StateManager stateManager) : State(stateManager)
    {
        public override async Task EnterFromAsync(LauncherBase launcher, State fromState, TimeSpan timeout)
        {
            if (!StateManager.TryEnter(launcher, fromState, this))
            {
                // Delegate KillAsync to current state, because it has already changed since
                // transition to this state was initiated.
                await StateManager.CurrentState.KillAsync(launcher).ConfigureAwait(false);
            }

            try
            {
                if (!launcher.Process.HasExited)
                {
                    launcher.Process.Kill();
                }
            }
            catch (InvalidOperationException)
            {
                // Ignore
                return;
            }

            await WaitForExitAsync(launcher).ConfigureAwait(false);
        }

        public override Task ExitAsync(LauncherBase p, TimeSpan timeout) => WaitForExitAsync(p);

        public override Task KillAsync(LauncherBase p) => WaitForExitAsync(p);
    }
}
