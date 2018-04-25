using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Tracing
    {
        private Session _client;
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

        public Tracing(Session client)
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
                Console.WriteLine("Cannot start recording trace while already recording trace.");
            }

            if (string.IsNullOrEmpty(options.Path))
            {
                Console.WriteLine("Must specify a path to write trace file to.");
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
                categories = string.Join(", ", categories.ToArray())
            });
        }

        /// <summary>
        /// Stops tracing
        /// </summary>
        /// <returns>Stop task</returns>
        public async Task StopAsync()
        {
            var taskWrapper = new TaskCompletionSource<bool>();

            void EventHandler(object sender, TracingCompleteEventArgs e)
            {
                using (var fs = new FileStream(_path, FileMode.Create, FileAccess.Write))
                {
                    byte[] bytesInStream = new byte[e.Stream.Length];
                    e.Stream.Read(bytesInStream, 0, bytesInStream.Length);
                    fs.Write(bytesInStream, 0, bytesInStream.Length);
                }

                _client.TracingComplete -= EventHandler;
                taskWrapper.SetResult(true);
            }

            _client.TracingComplete += EventHandler;

            await _client.SendAsync("Tracing.end");

            _recording = false;

            await taskWrapper.Task;
        }
    }
}