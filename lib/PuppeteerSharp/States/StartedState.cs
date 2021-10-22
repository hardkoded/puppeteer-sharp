using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.States
{
    internal class StartedState : State
    {
        public StartedState(StateManager stateManager) : base(stateManager)
        {
        }

        public override Task EnterFromAsync(LauncherBase p, State fromState, TimeSpan timeout)
        {
            StateManager.TryEnter(p, fromState, this);
            return Task.CompletedTask;
        }

        public override Task StartAsync(LauncherBase p) => Task.CompletedTask;

        public override Task ExitAsync(LauncherBase p, TimeSpan timeout) => new ExitingState(StateManager).EnterFromAsync(p, this, timeout);

        public override Task KillAsync(LauncherBase p) => new KillingState(StateManager).EnterFromAsync(p, this);
    }
}
