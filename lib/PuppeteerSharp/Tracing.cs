using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;

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
        public Task StartAsync(TracingOptions options = null)
        {
            if (_recording)
            {
                throw new InvalidOperationException("Cannot start recording trace while already recording trace.");
            }

            var categories = options?.Categories ?? _defaultCategories;

            if (options?.Screenshots == true)
            {
                categories.Add("disabled-by-default-devtools.screenshot");
            }

            _path = options?.Path;
            _recording = true;

            return _client.SendAsync("Tracing.start", new TracingStartRequest
            {
                TransferMode = "ReturnAsStream",
                Categories = string.Join(", ", categories),
            });
        }

        /// <summary>
        /// Stops tracing.
        /// </summary>
        /// <returns>Stop task.</returns>
        public async Task<string> StopAsync()
        {
            var taskWrapper = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            async void EventHandler(object sender, MessageEventArgs e)
            {
                try
                {
                    if (e.MessageID == "Tracing.tracingComplete")
                    {
                        var stream = e.MessageData.ToObject<TracingCompleteResponse>().Stream;
                        var tracingData = await ProtocolStreamReader.ReadProtocolStreamStringAsync(_client, stream, _path).ConfigureAwait(false);

                        _client.MessageReceived -= EventHandler;
                        taskWrapper.TrySetResult(tracingData);
                    }
                }
                catch (Exception ex)
                {
                    var message = $"Tracing failed to process the tracing complete. {ex.Message}. {ex.StackTrace}";
                    _logger.LogError(ex, message);
                    _client.Close(message);
                }
            }

            _client.MessageReceived += EventHandler;

            await _client.SendAsync("Tracing.end").ConfigureAwait(false);

            _recording = false;

            return await taskWrapper.Task.ConfigureAwait(false);
        }

        internal void UpdateClient(CDPSession newSession) => _client = newSession;
    }
}
