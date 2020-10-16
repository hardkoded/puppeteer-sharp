using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace PuppeteerSharp.Helpers
{
    /// <summary>
    /// TODO: doc me
    /// </summary>
    internal class TaskQueue2 : IDisposable
    {
        private readonly ConcurrentQueue<Func<Task>> _responseQueue;
        private readonly AutoResetEvent _enqueueSignal;
        private readonly ManualResetEvent _disposeSignal; // TODO: slim?
        private readonly ManualResetEvent _listenerDoneSignal;
        private readonly ILogger _logger;
        private bool _disposed;

        public TaskQueue2(ILogger logger = null)
        {
            _disposeSignal = new ManualResetEvent(false);
            _enqueueSignal = new AutoResetEvent(false);
            _listenerDoneSignal = new ManualResetEvent(false);
            _responseQueue = new ConcurrentQueue<Func<Task>>();
            _logger = logger ?? NullLogger.Instance;

            Task.Factory.StartNew(ListenQueue, TaskCreationOptions.LongRunning)
                .ContinueWith(t => _logger.LogError(t.Exception, "Unhandled error running task queue"), TaskContinuationOptions.OnlyOnFaulted);
        }

        public void Enqueue(Func<Task> task)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            _responseQueue.Enqueue(task);
            _enqueueSignal.Set();
        }

        public async Task FlushAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            while (!cancellationToken.IsCancellationRequested && _responseQueue.TryDequeue(out var taskFunc))
            {
                // TODO: try/catch?
                var task = taskFunc();
                _logger.LogTrace("Handling task {task}", task);
                await task.ConfigureAwait(false);
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _logger.LogTrace("Ending listener loop");
            WaitHandle.SignalAndWait(_disposeSignal, _listenerDoneSignal);

            _enqueueSignal.Dispose();
            _disposeSignal.Dispose();
            _listenerDoneSignal.Dispose();

            _disposed = true;
        }

        private async Task ListenQueue()
        {
            var signals = new WaitHandle[] {_disposeSignal, _enqueueSignal};

            try
            {
                while (true)
                {
                    WaitHandle nextSignal;
                    try
                    {
                        nextSignal = signals[WaitHandle.WaitAny(signals)];
                    }
                    catch (ObjectDisposedException)
                    {
                        // Unlikely scenario where we created this ResponseQueue and immediately disposed it before the
                        // listener task could get to this point. Just ignore and shut down the listener task.
                        return;
                    }

                    if (nextSignal == _disposeSignal)
                    {
                        // Dispose() was called. Stop looping.
                        _logger.LogTrace("Got disposal signal - ending listener loop");
                        return;
                    }

                    // Drain the queue async, so we can keep listening for new responses.
                    await FlushAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                _logger.LogTrace("Listener loop is done");
                _listenerDoneSignal.Set();
            }
        }
    }
}
