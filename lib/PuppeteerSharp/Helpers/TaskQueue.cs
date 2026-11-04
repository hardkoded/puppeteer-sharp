using System;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Helpers
{
    internal sealed class TaskQueue : IDisposable, IAsyncDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly AsyncLocal<bool> _held = new();
        private int _disposed;

        internal TaskQueue() => _semaphore = new SemaphoreSlim(1);

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            if (!_held.Value)
            {
                // Wait briefly for an in-flight task to release the semaphore. If a task
                // is still running when teardown is requested (e.g. Connection.Dispose
                // called from the user thread while a message is mid-process), we don't
                // want to block forever. In-flight tasks tolerate a disposed semaphore
                // via TryRelease catching ObjectDisposedException.
                _semaphore.Wait(TimeSpan.FromSeconds(1));
            }

            _semaphore.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            if (!_held.Value)
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
            }

            _semaphore.Dispose();
        }

        internal async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                _held.Value = true;
                return await taskGenerator().ConfigureAwait(false);
            }
            finally
            {
                TryRelease(_semaphore);
                _held.Value = false;
            }
        }

        internal async Task Enqueue(Func<Task> taskGenerator)
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                _held.Value = true;
                await taskGenerator().ConfigureAwait(false);
            }
            finally
            {
                TryRelease(_semaphore);
                _held.Value = false;
            }
        }

        private void TryRelease(SemaphoreSlim semaphore)
        {
            try
            {
                semaphore.Release();
            }
            catch (ObjectDisposedException)
            {
                // If semaphore has already been disposed, then Release() will fail
                // but we can safely ignore it
            }
        }
    }
}
