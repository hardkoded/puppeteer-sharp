using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// You can use <see cref="ITracing.StartAsync(TracingOptions)"/> and <see cref="ITracing.StopAsync"/> to create a trace file which can be opened in Chrome DevTools or timeline viewer.
    /// </summary>
    /// <example>
    /// <code>
    /// await Page.Tracing.StartAsync(new TracingOptions
    /// {
    ///     Screenshots = true,
    ///     Path = _file
    /// });
    /// await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
    /// await Page.Tracing.StopAsync();
    /// </code>
    /// </example>
#pragma warning disable CA1724
    public class Tracing : ITracing
#pragma warning restore CA1724
    {
        private static readonly List<string> _defaultCategories = new List<string>
        {
            "-*",
            "devtools.timeline",
            "v8.execute",
            "disabled-by-default-devtools.timeline",
            "disabled-by-default-devtools.timeline.frame",
            "toplevel",
            "blink.console",
            "blink.user_timing",
            "latencyInfo",
            "disabled-by-default-devtools.timeline.stack",
            "disabled-by-default-v8.cpu_profiler",
        };

        private readonly ILogger _logger;
        private bool _recording;
        private string _path;
        private CDPSession _client;

        internal Tracing(CDPSession client)
        {
            _client = client;
            _logger = client.LoggerFactory.CreateLogger<Tracing>();
        }

        /// <summary>
        /// Starts tracing.
        /// </summary>
        /// <returns>Start task.</returns>
        /// <param name="options">Tracing options.</param>
        public async Task StartAsync(TracingOptions options = null)
        {
            if (_recording)
            {
                throw new InvalidOperationException(
                    "Cannot start recording trace while already recording trace."
                );
            }

            var categories = new List<string>(options?.Categories ?? _defaultCategories);

            if (options?.Screenshots == true)
            {
                categories.Add("disabled-by-default-devtools.screenshot");
            }

            var excludedCategories = categories
                .FindAll(category => category.StartsWith("-", StringComparison.Ordinal))
                .ConvertAll(category => category.Substring(1));
            var includedCategories = categories.FindAll(category =>
                !category.StartsWith("-", StringComparison.Ordinal)
            );

            _path = options?.Path;
            _recording = true;

            try
            {
                await _client
                    .SendAsync(
                        "Tracing.start",
                        new TracingStartRequest
                        {
                            TransferMode = "ReturnAsStream",
                            TraceConfig = new TracingTraceConfig
                            {
                                ExcludedCategories = excludedCategories.ToArray(),
                                IncludedCategories = includedCategories.ToArray(),
                                TraceBufferSizeInKb = options?.BufferSize,
                            },
                        }
                    )
                    .ConfigureAwait(false);
            }
            catch
            {
                _recording = false;
                _path = null;
                throw;
            }
        }

        /// <summary>
        /// Stops tracing.
        /// </summary>
        /// <returns>Stop task.</returns>
        public async Task<string> StopAsync()
        {
            var taskWrapper = new TaskCompletionSource<string>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );

            // The page may swap its session while we are stopping, so stick to the current one.
            var client = _client;

            var disconnected = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );
            var traceFileClosed = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );

            // Set by whoever comes first: the handler, before it opens the trace file, or the end of
            // this method, so that the handler can't open the file afterwards.
            var traceFileClaimed = 0;

            async void OnMessageReceived(object sender, MessageEventArgs e)
            {
                if (
                    e.MessageID != "Tracing.tracingComplete"
                    || Interlocked.Exchange(ref traceFileClaimed, 1) == 1
                )
                {
                    return;
                }

                client.MessageReceived -= OnMessageReceived;

                try
                {
                    var stream = e.MessageData.ToObject<TracingCompleteResponse>().Stream;
                    string tracingData;

                    // The file must be closed before the caller of StopAsync gets the result.
                    using (
                        var outputStream = !string.IsNullOrEmpty(_path)
                            ? AsyncFileHelper.CreateStream(_path, FileMode.Create)
                            : null
                    )
                    {
                        tracingData = await ProtocolStreamReader
                            .ReadProtocolStreamStringAsync(
                                client,
                                stream,
                                outputStream,
                                disconnected.Task
                            )
                            .ConfigureAwait(false);
                    }

                    taskWrapper.TrySetResult(tracingData);
                }
                catch (Exception ex)
                {
                    var message =
                        $"Tracing failed to process the tracing complete. {ex.Message}. {ex.StackTrace}";
                    _logger.ErrorWithException(ex, message);
                    taskWrapper.TrySetException(ex);
                    client.Close(message);
                }
                finally
                {
                    traceFileClosed.TrySetResult(true);
                }
            }

            void OnDisconnected(object sender, EventArgs e)
            {
                taskWrapper.TrySetException(
                    new TargetClosedException("Tracing failed: Target closed.", client.CloseReason)
                );
                disconnected.TrySetResult(true);
            }

            client.MessageReceived += OnMessageReceived;
            client.Disconnected += OnDisconnected;

            try
            {
                try
                {
                    await client.SendAsync("Tracing.end").ConfigureAwait(false);
                }
                finally
                {
                    _recording = false;
                }

                return await taskWrapper.Task.ConfigureAwait(false);
            }
            finally
            {
                client.MessageReceived -= OnMessageReceived;

                // Don't return or throw while the handler has the trace file open.
                if (Interlocked.Exchange(ref traceFileClaimed, 1) == 1)
                {
                    await traceFileClosed.Task.ConfigureAwait(false);
                }

                // Removed last, because a disconnect is what stops the handler from waiting.
                client.Disconnected -= OnDisconnected;
            }
        }

        internal void UpdateClient(CDPSession newSession) => _client = newSession;
    }
}
