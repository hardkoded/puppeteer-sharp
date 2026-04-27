using System;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Helpers
{
    internal sealed class TaskQueue : IDisposable, IAsyncDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly AsyncLocal<bool> _held = new();
        private readonly CancellationTokenSource _disposeCts = new();
        private int _disposed;

        internal TaskQueue() => _semaphore = new SemaphoreSlim(1);

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            _disposeCts.Cancel();
            _disposeCts.Dispose();
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
            try
            {
                await _semaphore.WaitAsync(_disposeCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return default;
            }

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
            try
            {
                await _semaphore.WaitAsync(_disposeCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }

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

        private static void TryRelease(SemaphoreSlim semaphore)
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
