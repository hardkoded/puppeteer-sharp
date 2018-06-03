﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
        private static readonly List<string> _defaultCategories = new List<string>()
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

        internal Tracing(CDPSession client)
        {
            _client = client;
        }

        /// <summary>
        /// Starts tracing.
        /// </summary>
        /// <returns>Start task</returns>
        /// <param name="options">Tracing options</param>
        public async Task StartAsync(TracingOptions options)
        {
            if (_recording)
            {
                throw new InvalidOperationException("Cannot start recording trace while already recording trace.");
            }

            if (string.IsNullOrEmpty(options.Path))
            {
                throw new ArgumentException("Must specify a path to write trace file to.");
            }


            var categories = options.Categories ?? _defaultCategories;

            if (options.Screenshots)
            {
                categories.Add("disabled-by-default-devtools.screenshot");
            }

            _path = options.Path;
            _recording = true;

            await _client.SendAsync("Tracing.start", new
            {
                transferMode = "ReturnAsStream",
                categories = string.Join(", ", categories)
            });
        }

        /// <summary>
        /// Stops tracing
        /// </summary>
        /// <returns>Stop task</returns>
        public async Task StopAsync()
        {
            var taskWrapper = new TaskCompletionSource<bool>();

            async void EventHandler(object sender, TracingCompleteEventArgs e)
            {
                await ReadStream(e.Stream, _path);
                _client.TracingComplete -= EventHandler;
                taskWrapper.SetResult(true);
            }

            _client.TracingComplete += EventHandler;

            await _client.SendAsync("Tracing.end");

            _recording = false;

            await taskWrapper.Task;
        }

        private async Task ReadStream(string stream, string path)
        {
            using (var fs = new StreamWriter(path))
            {
                bool eof = false;

                while (!eof)
                {
                    var response = await _client.SendAsync<IOReadResponse>("IO.read", new
                    {
                        handle = stream
                    });

                    eof = response.Eof;

                    await fs.WriteAsync(response.Data);
                }
            }
            await _client.SendAsync("IO.close", new
            {
                handle = stream
            });
        }
    }
}