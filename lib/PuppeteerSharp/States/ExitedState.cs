using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.States
{
    internal class ExitedState(StateManager stateManager) : State(stateManager)
    {
        public void EnterFrom(LauncherBase launcher, State fromState)
        {
            while (!StateManager.TryEnter(launcher, fromState, this))
            {
                // Current state has changed since transition to this state was requested.
                // Therefore, retry transition to this state from the current state. This ensures
                // that Leave() operation of current state is properly called.
                fromState = StateManager.CurrentState;
                if (fromState == this)
                {
                    return;
                }
            }

            launcher.OnExit();
        }

        public override Task ExitAsync(LauncherBase p, TimeSpan timeout) => Task.CompletedTask;

        public override Task KillAsync(LauncherBase p) => Task.CompletedTask;

        public override Task WaitForExitAsync(LauncherBase p) => Task.CompletedTask;
    }
}
