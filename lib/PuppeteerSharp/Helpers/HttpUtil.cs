using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Helpers;

/// <summary>
/// Port of upstream's <c>httpUtil.ts</c> download helpers.
/// </summary>
internal static class HttpUtil
{
    private const int MaxRetryDelayInMillis = 10000;
    private const int CopyBufferSize = 81920;
    private static readonly TimeSpan _defaultBodyStallTimeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Downloads a file, retrying on transient failures (HTTP 408, 429, 5xx or network errors)
    /// with an exponential backoff capped at 10 seconds.
    /// </summary>
    /// <param name="address">The url to download.</param>
    /// <param name="destinationPath">Where to save the file.</param>
    /// <param name="proxy">Optional proxy.</param>
    /// <param name="retry">Number of retries. Defaults to 0.</param>
    /// <param name="retryDelay">Initial delay in milliseconds. Defaults to 1000.</param>
    /// <param name="requestTimeout">Optional request timeout. Defaults to the <see cref="HttpClient"/> default.</param>
    /// <param name="bodyStallTimeout">Maximum time to wait for the next chunk of the response body before the attempt fails as a transient error. Defaults to 30 seconds.</param>
    /// <returns>A task that resolves when the download finishes.</returns>
    internal static async Task DownloadFileAsync(
        string address,
        string destinationPath,
        IWebProxy proxy = null,
        int retry = 0,
        int retryDelay = 1000,
        TimeSpan? requestTimeout = null,
        TimeSpan? bodyStallTimeout = null
    )
    {
        var attempt = 0;
        while (true)
        {
            try
            {
                await DownloadFileAttemptAsync(
                        address,
                        destinationPath,
                        proxy,
                        requestTimeout,
                        bodyStallTimeout ?? _defaultBodyStallTimeout
                    )
                    .ConfigureAwait(false);
                return;
            }
            catch (Exception ex) when (IsTransientError(ex) && attempt < retry)
            {
                attempt++;
                var backoff =
                    retryDelay > 0
                        ? (int)
                            Math.Min(retryDelay * Math.Pow(2, attempt - 1), MaxRetryDelayInMillis)
                        : 0;
                if (backoff > 0)
                {
                    await Task.Delay(backoff).ConfigureAwait(false);
                }
            }
        }
    }

    private static bool IsTransientError(Exception error) =>
        error switch
        {
            DownloadError downloadError => downloadError.IsTransient,
            HttpRequestException or IOException => true,
            OperationCanceledException { InnerException: TimeoutException } => true,
            _ => false,
        };

    private static async Task DownloadFileAttemptAsync(
        string address,
        string destinationPath,
        IWebProxy proxy,
        TimeSpan? requestTimeout,
        TimeSpan bodyStallTimeout
    )
    {
        using var handler = new HttpClientHandler();
        if (proxy != null)
        {
            handler.Proxy = proxy;
            handler.UseProxy = true;
        }

        using var client = new HttpClient(handler);
        if (requestTimeout.HasValue)
        {
            client.Timeout = requestTimeout.Value;
        }

        using var response = await client
            .GetAsync(address, HttpCompletionOption.ResponseHeadersRead)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var code = (int)response.StatusCode;
            throw new DownloadError(
                $"Download failed: server returned code {code}. URL: {address}",
                code is 408 or 429 or (>= 500 and < 600)
            );
        }

        try
        {
            using var downloadStream = await response
                .Content.ReadAsStreamAsync()
                .ConfigureAwait(false);
            using var fileStream = File.Create(destinationPath);
            await CopyWithStallTimeoutAsync(downloadStream, fileStream, bodyStallTimeout, address)
                .ConfigureAwait(false);
        }
        catch
        {
            try
            {
                File.Delete(destinationPath);
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }

            throw;
        }
    }

    private static async Task CopyWithStallTimeoutAsync(
        Stream source,
        Stream destination,
        TimeSpan stallTimeout,
        string address
    )
    {
        var buffer = new byte[CopyBufferSize];
        using var cts = new CancellationTokenSource();
        while (true)
        {
            // The timeout restarts on every read, so only a stalled body fails, not a slow one.
            cts.CancelAfter(stallTimeout);
            int read;
            try
            {
                read = await source
                    .ReadAsync(buffer, 0, buffer.Length, cts.Token)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cts.IsCancellationRequested)
            {
                throw new OperationCanceledException(
                    $"Download stalled: no data received for {stallTimeout.TotalMilliseconds}ms. URL: {address}",
                    new TimeoutException()
                );
            }

            if (read == 0)
            {
                return;
            }

            await destination.WriteAsync(buffer, 0, read).ConfigureAwait(false);
        }
    }

    private sealed class DownloadError : HttpRequestException
    {
        public DownloadError() { }

        public DownloadError(string message)
            : base(message) { }

        public DownloadError(string message, Exception innerException)
            : base(message, innerException) { }

        public DownloadError(string message, bool isTransient)
            : base(message) => IsTransient = isTransient;

        public bool IsTransient { get; }
    }
}
