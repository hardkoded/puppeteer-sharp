using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Helpers
{
    internal class TaskQueue : IDisposable, IAsyncDisposable
    {
        [SuppressMessage("Usage", "CA2213: Disposable fields should be disposed", Justification = "The disposable field is being disposed asynchronously.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "The CA2213 suppression is actually necessary.")]
        private readonly SemaphoreSlim _semaphore;
        private bool _isDisposed;

        internal TaskQueue() => _semaphore = new SemaphoreSlim(1);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "Per MSDN instructions for implementing the IAsyncDisposable pattern.")]
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            Dispose(false);
            GC.SuppressFinalize(this);
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

        protected virtual void Dispose(bool dispose)
        {
            if (_isDisposed)
            {
                return;
            }

            if (dispose)
            {
                _ = Task.Run(() => DisposeAsync());
            }

            _isDisposed = true;
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            _semaphore.Dispose();
        }
    }
}
