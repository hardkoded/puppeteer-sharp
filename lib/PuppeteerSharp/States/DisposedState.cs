using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.States
{
    internal class DisposedState(StateManager stateManager) : State(stateManager)
    {
        public override Task EnterFromAsync(LauncherBase launcher, State fromState, TimeSpan timeout)
        {
            if (fromState == StateManager.Exited)
            {
                return Task.CompletedTask;
            }

            Kill(launcher);

            if (launcher.TempUserDataDir is { } tempUserDataDir)
            {
                tempUserDataDir
                    .DeleteAsync()
                    .ContinueWith(
                        t => launcher.ExitCompletionSource.TrySetException(new ObjectDisposedException(launcher.ToString())),
                        TaskScheduler.Default);
            }
            else
            {
                launcher.ExitCompletionSource.TrySetException(new ObjectDisposedException(launcher.ToString()));
            }

            return Task.CompletedTask;
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
