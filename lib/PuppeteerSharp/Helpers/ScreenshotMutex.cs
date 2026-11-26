using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Helpers
{
    internal sealed class ScreenshotMutex
    {
        private readonly Queue<TaskCompletionSource<bool>> _acquirers = new();
        private readonly object _lock = new();
        private bool _locked;

        // This is FIFO.
        internal async Task<IDisposable> AcquireAsync(Action onRelease = null)
        {
            TaskCompletionSource<bool> tcs = null;

            lock (_lock)
            {
                if (_locked)
                {
                    tcs = new TaskCompletionSource<bool>(
                        TaskCreationOptions.RunContinuationsAsynchronously
                    );
                    _acquirers.Enqueue(tcs);
                }
                else
                {
                    _locked = true;
                }
            }

            if (tcs != null)
            {
                await tcs.Task.ConfigureAwait(false);
            }

            return new Guard(this, onRelease);
        }

        private void Release()
        {
            lock (_lock)
            {
                if (_acquirers.Count > 0)
                {
                    var next = _acquirers.Dequeue();
                    next.TrySetResult(true);
                }
                else
                {
                    _locked = false;
                }
            }
        }

        private sealed class Guard : IDisposable
        {
            private readonly ScreenshotMutex _mutex;
            private readonly Action _onRelease;
            private int _disposed;

            internal Guard(ScreenshotMutex mutex, Action onRelease)
            {
                _mutex = mutex;
                _onRelease = onRelease;
            }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) != 0)
                {
                    return;
                }

                _onRelease?.Invoke();
                _mutex.Release();
            }
        }
    }
}
