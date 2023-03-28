using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Helpers
{
    internal sealed class TaskQueue : IDisposable, IAsyncDisposable
    {
        [SuppressMessage("Usage", "CA2213: Disposable fields should be disposed", Justification = "The disposable field is being disposed asynchronously.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "The CA2213 suppression is actually necessary.")]
        private readonly SemaphoreSlim _semaphore;
        private bool _isDisposed;

        internal TaskQueue() => _semaphore = new SemaphoreSlim(1);

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _ = Task.Run(() => DisposeAsync());

            _isDisposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
            }
            catch (ObjectDisposedException)
            {
                // Ignore
            }

            _semaphore.Dispose();
        }

        internal async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                return await taskGenerator().ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        internal async Task Enqueue(Func<Task> taskGenerator)
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                await taskGenerator().ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
