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

        private static async Task DeleteAsync(string path)
        {
            const int minDelayInMillis = 200;
            const int maxDelayInMillis = 8000;

            var retryDelay = minDelayInMillis;
            while (true)
            {
                if (!Directory.Exists(path))
                {
                    return;
                }

                try
                {
                    Directory.Delete(path, true);
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

        private void DisposeCore()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            _ = DeleteAsync(Path);
        }
    }
}
