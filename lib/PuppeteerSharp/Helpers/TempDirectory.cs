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
    internal class TempDirectory : IDisposable
    {
        private Task _deleteTask;

        public TempDirectory()
            : this(PathHelper.Combine(PathHelper.GetTempPath(), PathHelper.GetRandomFileName()))
        {
        }

        public TempDirectory(string path)
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
            Dispose(false);
        }

        public string Path { get; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (_deleteTask == null)
            {
                _ = DeleteAsync();
            }
        }

        public override string ToString() => Path;

        public Task DeleteAsync(CancellationToken cancellationToken = default)
            => _deleteTask ?? (_deleteTask = DeleteAsync(Path, CancellationToken.None));

        private static async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            const int minDelayInMsec = 200;
            const int maxDelayInMsec = 8000;

            int retryDelay = minDelayInMsec;
            while (true)
            {
                if (!Directory.Exists(path))
                {
                    return;
                }

                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    Directory.Delete(path, true);
                    return;
                }
                catch
                {
                    await Task.Delay(retryDelay, cancellationToken).ConfigureAwait(false);
                    if (retryDelay < maxDelayInMsec)
                    {
                        retryDelay = Math.Min(2 * retryDelay, maxDelayInMsec);
                    }
                }
            }
        }
    }
}
