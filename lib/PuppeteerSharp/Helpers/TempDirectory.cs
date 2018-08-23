namespace PuppeteerSharp.Helpers
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a directory that is deleted on disposal.
    /// </summary>
    internal class TempDirectory : IDisposable
    {
        private readonly string _path;
        private Task _deleteTask;

        public TempDirectory()
            : this(System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName()))
        { }

        public TempDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path must be specified", nameof(path));
            }

            Directory.CreateDirectory(path);
            _path = path;
        }

        ~TempDirectory()
        {
            Dispose(false);
        }

        public string Path
        {
            get => _path;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected async void Dispose(bool disposing)
        {
            if (_deleteTask == null)
            {
                await Delete().ConfigureAwait(false);
            }
        }

        public override string ToString()
        {
            return _path;
        }

        public Task Delete(CancellationToken cancellationToken = default)
        {
            return _deleteTask ?? (_deleteTask = Delete(_path, CancellationToken.None));
        }

        private static async Task Delete(string path, CancellationToken cancellationToken = default)
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
