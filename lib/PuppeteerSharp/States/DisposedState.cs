using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.States
{
    internal class DisposedState : State
    {
        public DisposedState(StateManager stateManager) : base(stateManager)
        {
        }

        public void EnterFrom(LauncherBase p, State fromState)
        {
            if (!StateManager.TryEnter(p, fromState, this))
            {
                // Delegate Dispose to current state, because it has already changed since
                // transition to this state was initiated.
                StateManager.CurrentState.Dispose(p);
            }
            else if (fromState != StateManager.Exited)
            {
                Kill(p);

                p.ExitCompletionSource.TrySetException(new ObjectDisposedException(p.ToString()));
                p.TempUserDataDir?.Dispose();
            }
        }

        public override Task StartAsync(LauncherBase p) => throw new ObjectDisposedException(p.ToString());

        public override Task ExitAsync(LauncherBase p, TimeSpan timeout) => throw new ObjectDisposedException(p.ToString());

        public override Task KillAsync(LauncherBase p) => throw new ObjectDisposedException(p.ToString());

        public override void Dispose(LauncherBase p)
        {
            // Nothing to do
        }
    }
}
