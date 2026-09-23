using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PathHelper = System.IO.Path;

namespace PuppeteerSharp.Helpers
{
    /// <summary>
    /// Represents a directory that is deleted on disposal.
    /// </summary>
    internal sealed class TempDirectory : IDisposable
    {
        private int _disposed;
        private Action _unregisterProcessExitCleanup;

        public TempDirectory()
            : this(PathHelper.Combine(PathHelper.GetTempPath(), PathHelper.GetRandomFileName()))
        {
        }

        private TempDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path must be specified", nameof(path));
            }

            Directory.CreateDirectory(path);
            Path = path;
        }

        ~TempDirectory()
        {
            DisposeCore();
        }

        public string Path { get; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            DisposeCore();
        }

        public override string ToString() => Path;

        /// <summary>
        /// Registers a synchronous host-process-exit fallback that deletes this
        /// temporary directory if async cleanup cannot run.
        /// </summary>
        public void RegisterProcessExitCleanup()
        {
            _unregisterProcessExitCleanup ??= ProcessExitCleanup.Register(Path);
        }

        public async Task DeleteAsync()
        {
            const int maxRetries = 10;
            const int retryDelayInMillis = 100;
            const int maxDelayInMillis = 8000;

            try
            {
                var retryDelay = retryDelayInMillis;
                var attempt = 0;
                while (Directory.Exists(Path))
                {
                    try
                    {
                        Directory.Delete(Path, true);
                        return;
                    }
                    catch
                    {
                        await Task.Delay(retryDelay).ConfigureAwait(false);
                        attempt++;
                        if (attempt >= maxRetries && retryDelay < maxDelayInMillis)
                        {
                            retryDelay = Math.Min(2 * retryDelay, maxDelayInMillis);
                        }
                    }
                }
            }
            finally
            {
                UnregisterProcessExitCleanup();
            }
        }

        private void UnregisterProcessExitCleanup()
        {
            var unregister = Interlocked.Exchange(ref _unregisterProcessExitCleanup, null);
            unregister?.Invoke();
        }

        private void DisposeCore()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            _ = DeleteAsync();
        }
    }
}
