using System;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.States
{
    /// <summary>
    /// Represents state machine for Base process instances. The happy path runs along the
    /// following state transitions: <see cref="StateManager.Initial"/>
    /// -> <see cref="StateManager.Starting"/>
    /// -> <see cref="StateManager.Started"/>
    /// -> <see cref="StateManager.Exiting"/>
    /// -> <see cref="StateManager.Exited"/>.
    /// -> <see cref="StateManager.Disposed"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This state machine implements the following state transitions:
    /// <code>
    /// State     Event              Target State Action
    /// ======== =================== ============ ==========================================================
    /// Initial  --StartAsync------> Starting     Start process and wait for endpoint
    /// Initial  --ExitAsync-------> Exited       Cleanup temp user data
    /// Initial  --KillAsync-------> Exited       Cleanup temp user data
    /// Initial  --Dispose---------> Disposed     Cleanup temp user data
    /// Starting --StartAsync------> Starting     -
    /// Starting --ExitAsync-------> Exiting      Wait for process exit
    /// Starting --KillAsync-------> Killing      Kill process
    /// Starting --Dispose---------> Disposed     Kill process; Cleanup temp user data;  throw ObjectDisposedException on outstanding async operations;
    /// Starting --endpoint ready--> Started      Complete StartAsync successfully; Log process start
    /// Starting --process exit----> Exited       Complete StartAsync with exception; Cleanup temp user data
    /// Started  --StartAsync------> Started      -
    /// Started  --EnsureExitAsync-> Exiting      Start exit timer; Log process exit
    /// Started  --KillAsync-------> Killing      Kill process; Log process exit
    /// Started  --Dispose---------> Disposed     Kill process; Log process exit; Cleanup temp user data; throw ObjectDisposedException on outstanding async operations;
    /// Started  --process exit----> Exited       Log process exit; Cleanup temp user data
    /// Exiting  --StartAsync------> Exiting      - (StartAsync throws InvalidOperationException)
    /// Exiting  --ExitAsync-------> Exiting      -
    /// Exiting  --KillAsync-------> Killing      Kill process
    /// Exiting  --Dispose---------> Disposed     Kill process; Cleanup temp user data; throw ObjectDisposedException on outstanding async operations;
    /// Exiting  --exit timeout----> Killing      Kill process
    /// Exiting  --process exit----> Exited       Cleanup temp user data; complete outstanding async operations;
    /// Killing  --StartAsync------> Killing      - (StartAsync throws InvalidOperationException)
    /// Killing  --KillAsync-------> Killing      -
    /// Killing  --Dispose---------> Disposed     Cleanup temp user data; throw ObjectDisposedException on outstanding async operations;
    /// Killing  --process exit----> Exited       Cleanup temp user data; complete outstanding async operations;
    /// Exited   --StartAsync------> Killing      - (StartAsync throws InvalidOperationException)
    /// Exited   --KillAsync-------> Exited       -
    /// Exited   --Dispose---------> Disposed     -
    /// Disposed --StartAsync------> Disposed     -
    /// Disposed --KillAsync-------> Disposed     -
    /// Disposed --Dispose---------> Disposed     -
    /// </code>
    /// </para>
    /// </remarks>
    internal abstract class State
    {
        public State(StateManager stateManager)
        {
            StateManager = stateManager;
        }

        public StateManager StateManager { get; set; }

        public bool IsExiting => this == StateManager.Killing || this == StateManager.Exiting;

        public bool IsExited => this == StateManager.Exited || this == StateManager.Disposed;

        public virtual Task EnterFromAsync(LauncherBase p, State fromState) => EnterFromAsync(p, fromState, TimeSpan.Zero);

        public virtual Task EnterFromAsync(LauncherBase p, State fromState, TimeSpan timeout) => Task.FromException(InvalidOperation("enterFrom"));

        public virtual Task StartAsync(LauncherBase p) => Task.FromException(InvalidOperation("start"));

        public virtual Task ExitAsync(LauncherBase p, TimeSpan timeout) => Task.FromException(InvalidOperation("exit"));

        public virtual Task KillAsync(LauncherBase p) => Task.FromException(InvalidOperation("kill"));

        public virtual Task WaitForExitAsync(LauncherBase p) => p.ExitCompletionSource.Task;

        public virtual void Dispose(LauncherBase p) => StateManager.Disposed.EnterFromAsync(p, this, TimeSpan.Zero);

        public override string ToString()
        {
            var name = GetType().Name;
            return name.Substring(0, name.Length - "State".Length);
        }

        internal virtual void Leave(LauncherBase p)
        {
        }

        protected static void Kill(LauncherBase p)
        {
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
            }
        }

        private Exception InvalidOperation(string operationName)
            => new InvalidOperationException($"Cannot {operationName} in state {this}");
    }
}
