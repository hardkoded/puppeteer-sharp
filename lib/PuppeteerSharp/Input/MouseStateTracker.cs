using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp.Input
{
    internal class MouseStateTracker
    {
        private readonly object _lock = new();
        private readonly MouseState _committedState = new();

        // Transactions can run in parallel, so we store each of them in this list.
        private readonly List<MouseTransaction.TransactionData> _transactions = [];

        // Continuations can run out of order, so we remember which transaction committed each value.
        private long _lastSequence;
        private long _positionSequence;
        private long _buttonsSequence;

        /// <summary>
        /// Gets the current position of the mouse.
        /// </summary>
        internal Point Position => State.Position;

        /// <summary>
        /// Gets the buttons that are currently being pressed.
        /// </summary>
        internal MouseButton Buttons => State.Buttons;

        /// <summary>
        /// Gets a snapshot of the committed state with the updates of the in-flight transactions applied on top of it.
        /// </summary>
        internal MouseState State
        {
            get
            {
                lock (_lock)
                {
                    var state = new MouseState
                    {
                        Position = _committedState.Position,
                        Buttons = _committedState.Buttons,
                    };

                    foreach (var transaction in _transactions)
                    {
                        if (transaction.Sequence > _positionSequence)
                        {
                            state.Position = transaction.Position ?? state.Position;
                        }

                        if (transaction.Sequence > _buttonsSequence)
                        {
                            state.Buttons = transaction.Buttons ?? state.Buttons;
                        }
                    }

                    return state;
                }
            }
        }

        /// <summary>
        /// This is a shortcut for a typical update, commit/rollback lifecycle based on
        /// the error of the action.
        /// </summary>
        /// <param name="action">Action that updates the state and sends the command to the browser.</param>
        /// <returns>A task that completes when the action completes.</returns>
        internal async Task WithTransactionAsync(
            Func<Action<MouseTransaction.TransactionData>, Task> action
        )
        {
            MouseTransaction transaction = null;
            try
            {
                Task task;

                // The synchronous part of the action updates the state and sends the command.
                // Holding the lock makes both happen in the same order for all callers.
                lock (_lock)
                {
                    transaction = CreateTransaction();
                    task = action(transaction.Update);
                }

                await task.ConfigureAwait(false);
                transaction.Commit();
            }
            catch
            {
                transaction?.Rollback();
                throw;
            }
        }

        private MouseTransaction CreateTransaction()
        {
            var data = new MouseTransaction.TransactionData { Sequence = ++_lastSequence };
            _transactions.Add(data);

            return new MouseTransaction()
            {
                Update = updates =>
                {
                    lock (_lock)
                    {
                        data.Position = updates.Position ?? data.Position;
                        data.Buttons = updates.Buttons ?? data.Buttons;
                    }
                },
                Commit = () =>
                {
                    lock (_lock)
                    {
                        if (data.Position.HasValue && data.Sequence > _positionSequence)
                        {
                            _committedState.Position = data.Position.Value;
                            _positionSequence = data.Sequence;
                        }

                        if (data.Buttons.HasValue && data.Sequence > _buttonsSequence)
                        {
                            _committedState.Buttons = data.Buttons.Value;
                            _buttonsSequence = data.Sequence;
                        }

                        _transactions.Remove(data);
                    }
                },
                Rollback = () =>
                {
                    lock (_lock)
                    {
                        _transactions.Remove(data);
                    }
                },
            };
        }
    }
}
