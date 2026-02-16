using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.States
{
    internal class InitialState(StateManager stateManager) : State(stateManager)
    {
        public override Task StartAsync(LauncherBase p)
            => StateManager.Starting.EnterFromAsync(p, this, TimeSpan.Zero);

        public override Task ExitAsync(LauncherBase launcher, TimeSpan timeout)
        {
            StateManager.Exited.EnterFromAsync(launcher, this);
            return Task.CompletedTask;
        }

        public override Task KillAsync(LauncherBase p)
        {
            StateManager.Exited.EnterFromAsync(p, this);
            return Task.CompletedTask;
        }

        public override Task WaitForExitAsync(LauncherBase p)
            => Task.FromException(new InvalidOperationException("wait for exit"));
    }
}
