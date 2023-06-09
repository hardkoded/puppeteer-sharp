using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Helpers
{
    /// <summary>
    /// An async queue that accepts a task but defers its execution to be handled by a consumer queue.
    /// </summary>
    internal class DeferredTaskQueue
    {
        private readonly List<Task> _pendingTasks = new();

        public async Task Enqueue(Func<Task> taskGenerator)
        {
            var task = taskGenerator();
            if (task.IsCompleted)
            {
                // Don't need to do anything.
                return;
            }

            try
            {
                lock (_pendingTasks)
                {
                    _pendingTasks.Add(task);
                }

                await task.ConfigureAwait(false);
            }
            finally
            {
                lock (_pendingTasks)
                {
                    _pendingTasks.Remove(task);
                }
            }
        }

        public async Task DrainAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Task task;
                lock (_pendingTasks)
                {
                    if (_pendingTasks.Count == 0)
                    {
                        break;
                    }

                    task = _pendingTasks[0];
                }

                try
                {
                    await task.ConfigureAwait(false);
                }
                finally
                {
                    lock (_pendingTasks)
                    {
                        _pendingTasks.Remove(task);
                    }
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}
