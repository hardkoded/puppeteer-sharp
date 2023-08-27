using System;
using System.Threading;

namespace PuppeteerSharp.States
{
    internal class StateManager
    {
        private State _currentState;

        public StateManager()
        {
            Initial = new InitialState(this);
            Starting = new ProcessStartingState(this);
            Started = new StartedState(this);
            Exiting = new ExitingState(this);
            Killing = new KillingState(this);
            Exited = new ExitedState(this);
            Disposed = new DisposedState(this);
            CurrentState = Initial;
        }

        public State CurrentState
        {
            get => _currentState;
            set => _currentState = value;
        }

        internal State Initial { get; set; }

        internal State Starting { get; set; }

        internal StartedState Started { get; set; }

        internal State Exiting { get; set; }

        internal State Killing { get; set; }

        internal ExitedState Exited { get; set; }

        internal State Disposed { get; set; }

        public bool TryEnter(LauncherBase p, State fromState, State toState)
        {
            if (Interlocked.CompareExchange(ref _currentState, toState, fromState) == fromState)
            {
                fromState.Leave(p);
                return true;
            }

            return false;
        }
    }
}
