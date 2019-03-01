namespace PuppeteerSharp
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal interface IStateMachine<TState>
    {
        TState CurrentState { get; }
        bool TryEnter(TState newState, TState fromState);
    }

    /// <summary>
    /// Base class for states.
    /// </summary>
    internal abstract class AbstractState<TContext, TState>
        where TContext : IStateMachine<TState>
        where TState : AbstractState<TContext, TState>
    {
        /// <summary>
        /// Attempts thread-safe transitions from a given state to this state.
        /// </summary>
        /// <param name="context">The state machine</param>
        /// <param name="fromState">The state from which state transition takes place</param>
        /// <returns>Returns <c>true</c> if transition is successful, or <c>false</c> if transition
        /// cannot be made because current state does not equal <paramref name="fromState"/>.</returns>
        protected bool TryEnter(TContext context, TState fromState)
        {
            if (context.TryEnter((TState)this, fromState))
            {
                fromState.Leave(context);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Notifies that state machine is about to transition to another state.
        /// </summary>
        /// <param name="context">The state machine</param>
        protected virtual void Leave(TContext context)
        { }

        /// <inheritdoc />
        public override string ToString()
        {
            var name = GetType().Name;
            return name.Substring(0, name.Length - "State".Length);
        }

        /// <summary>
        /// Creates a <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="operationName">The operation name, defaults to name of direct caller.</param>
        /// <returns>An <see cref="InvalidOperationException"/> for the specified <paramref name="operationName"/>.</returns>
        protected Exception InvalidOperation([CallerMemberName] string operationName = null)
            => new InvalidOperationException($"Cannot {operationName.Replace("Async", null)} in state {this}");
    }
}
