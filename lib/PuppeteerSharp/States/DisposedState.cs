using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.States
{
    internal class DisposedState : State
    {
        public DisposedState(StateManager stateManager) : base(stateManager)
        {
        }

        public override Task EnterFromAsync(LauncherBase p, State fromState, TimeSpan timeSpan)
        {
            if (fromState == StateManager.Exited)
            {
                return null;
            }

            Kill(p);

            p.ExitCompletionSource.TrySetException(new ObjectDisposedException(p.ToString()));
            p.TempUserDataDir?.Dispose();

            return null;
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
