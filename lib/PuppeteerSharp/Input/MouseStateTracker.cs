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

        /// <summary>
        /// Gets the current position of the mouse.
        /// </summary>
        internal Point Position => State.Position;

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
                        Apply(transaction, state);
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

        private static void Apply(MouseTransaction.TransactionData transaction, MouseState state)
        {
            state.Position = transaction.Position ?? state.Position;
            state.Buttons =
                (state.Buttons | transaction.PressedButtons) & ~transaction.ReleasedButtons;
        }

        // .NET continuations can run out of order, so transactions are applied in the order they were created.
        private void ApplyCommittedTransactions()
        {
            while (_transactions.Count > 0 && _transactions[0].IsCommitted)
            {
                Apply(_transactions[0], _committedState);
                _transactions.RemoveAt(0);
            }
        }

        private MouseTransaction CreateTransaction()
        {
            var data = new MouseTransaction.TransactionData();
            _transactions.Add(data);

            return new MouseTransaction()
            {
                Update = updates =>
                {
                    lock (_lock)
                    {
                        data.Position = updates.Position ?? data.Position;
                        data.PressedButtons |= updates.PressedButtons;
                        data.ReleasedButtons |= updates.ReleasedButtons;
                    }
                },
                Commit = () =>
                {
                    lock (_lock)
                    {
                        data.IsCommitted = true;
                        ApplyCommittedTransactions();
                    }
                },
                Rollback = () =>
                {
                    lock (_lock)
                    {
                        _transactions.Remove(data);
                        ApplyCommittedTransactions();
                    }
                },
            };
        }
    }
}
