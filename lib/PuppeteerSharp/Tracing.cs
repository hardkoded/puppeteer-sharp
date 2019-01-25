using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// You can use <see cref="Tracing.StartAsync(TracingOptions)"/> and <see cref="Tracing.StopAsync"/> to create a trace file which can be opened in Chrome DevTools or timeline viewer.
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
    public class Tracing
    {
        private readonly CDPSession _client;
        private bool _recording;
        private string _path;
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
            "disabled-by-default-v8.cpu_profiler"
        };
        private readonly ILogger _logger;

        internal Tracing(CDPSession client)
        {
            _client = client;
            _logger = client.LoggerFactory.CreateLogger<Tracing>();
        }

        /// <summary>
        /// Starts tracing.
        /// </summary>
        /// <returns>Start task</returns>
        /// <param name="options">Tracing options</param>
        public Task StartAsync(TracingOptions options)
        {
            if (_recording)
            {
                throw new InvalidOperationException("Cannot start recording trace while already recording trace.");
            }

            var categories = options.Categories ?? _defaultCategories;

            if (options.Screenshots)
            {
                categories.Add("disabled-by-default-devtools.screenshot");
            }

            _path = options.Path;
            _recording = true;

            return _client.SendAsync("Tracing.start", new TracingStartRequest
            {
                TransferMode = "ReturnAsStream",
                Categories = string.Join(", ", categories)
            });
        }

        /// <summary>
        /// Stops tracing
        /// </summary>
        /// <returns>Stop task</returns>
        public async Task<string> StopAsync()
        {
            var taskWrapper = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            async void EventHandler(object sender, TracingCompleteEventArgs e)
            {
                try
                {
                    var tracingData = await ReadStream(e.Stream, _path).ConfigureAwait(false);
                    _client.TracingComplete -= EventHandler;
                    taskWrapper.TrySetResult(tracingData);
                }
                catch (Exception ex)
                {
                    var message = $"Tracing failed to process the tracing complete. {ex.Message}. {ex.StackTrace}";
                    _logger.LogError(ex, message);
                    _client.Close(message);
                }
            }

            _client.TracingComplete += EventHandler;

            await _client.SendAsync("Tracing.end").ConfigureAwait(false);

            _recording = false;

            return await taskWrapper.Task.ConfigureAwait(false);
        }

        private async Task<string> ReadStream(string stream, string path)
        {
            var result = new StringBuilder();
            var eof = false;

            while (!eof)
            {
                var response = await _client.SendAsync<IOReadResponse>("IO.read", new IOReadRequest
                {
                    Handle = stream
                }).ConfigureAwait(false);

                eof = response.Eof;

                result.Append(response.Data);
            }

            if (!string.IsNullOrEmpty(path))
            {
                using (var fs = new StreamWriter(path))
                {
                    await fs.WriteAsync(result.ToString()).ConfigureAwait(false);
                }
            }

            await _client.SendAsync("IO.close", new IOCloseRequest
            {
                Handle = stream
            }).ConfigureAwait(false);

            return result.ToString();
        }
    }
}