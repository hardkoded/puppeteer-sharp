using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Helpers
{
    internal sealed class ScreenshotMutex
    {
        private readonly Queue<TaskCompletionSource<bool>> _acquirers = new();
        private bool _locked;

        internal async Task<IDisposable> AcquireAsync(Action onRelease = null)
        {
            if (!_locked)
            {
                _locked = true;
                return new Guard(this, onRelease);
            }

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _acquirers.Enqueue(tcs);
            await tcs.Task.ConfigureAwait(false);
            return new Guard(this, onRelease);
        }

        private void Release()
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
