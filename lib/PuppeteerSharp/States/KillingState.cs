using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.States
{
    internal class KillingState : State
    {
        public KillingState(StateManager stateManager) : base(stateManager)
        {
        }

        public override async Task EnterFromAsync(LauncherBase p, State fromState, TimeSpan timeout)
        {
            if (!StateManager.TryEnter(p, fromState, this))
            {
                // Delegate KillAsync to current state, because it has already changed since
                // transition to this state was initiated.
                await StateManager.CurrentState.KillAsync(p).ConfigureAwait(false);
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
}
