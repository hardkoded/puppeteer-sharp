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
    internal sealed class TempDirectory : IDisposable, IAsyncDisposable
    {
        private int _disposed;
        private Task _deleteTask;

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

        ~TempDirectory() => Dispose();

        public string Path { get; }

#pragma warning disable CA1816
        public void Dispose() => _ = DisposeAsync();
#pragma warning restore CA1816

        public override string ToString() => Path;

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            _deleteTask ??= DeleteAsync();
            await _deleteTask.ConfigureAwait(false);
        }

        private async Task DeleteAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            const int minDelayInMillis = 200;
            const int maxDelayInMillis = 8000;

            var retryDelay = minDelayInMillis;
            while (true)
            {
                if (!Directory.Exists(Path))
                {
                    return;
                }

                try
                {
                    Directory.Delete(Path, true);
                    return;
                }
                catch
                {
                    await Task.Delay(retryDelay).ConfigureAwait(false);
                    if (retryDelay < maxDelayInMillis)
                    {
                        retryDelay = Math.Min(2 * retryDelay, maxDelayInMillis);
                    }
                }
            }
        }
    }
}
