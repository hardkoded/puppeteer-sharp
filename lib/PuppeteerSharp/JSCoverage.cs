using PuppeteerSharp.Messaging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    internal class JSCoverage
    {
        private readonly Session _client;
        private readonly Dictionary<string, string> _scriptURLs;
        private readonly Dictionary<string, string> _scriptSources;

        private bool _enabled;
        private bool _resetOnNavigation;

        public JSCoverage(Session client)
        {
            _client = client;
            _enabled = false;
            _scriptURLs = new Dictionary<string, string>();
            _scriptSources = new Dictionary<string, string>();

            _resetOnNavigation = false;
        }

        internal async Task StartAsync(JSCoverageStartOptions options)
        {
            if (_enabled)
            {
                throw new InvalidOperationException("JSCoverage is already enabled");
            }

            _resetOnNavigation = options.ResetOnNavigation;
            _enabled = true;
            _scriptURLs.Clear();
            _scriptSources.Clear();

            _client.MessageReceived += client_MessageReceived;

            await Task.WhenAll(
                _client.SendAsync("Profiler.enable"),
                _client.SendAsync("Profiler.startPreciseCoverage", new { callCount = false, detailed = true }),
                _client.SendAsync("Debugger.enable"),
                _client.SendAsync("Debugger.setSkipAllPauses", new { skip = true })
            );
        }

        internal async Task<CoverageEntry[]> StopAsync()
        {
            if (!_enabled)
            {
                throw new InvalidOperationException("JSCoverage is not enabled");
            }
            _enabled = false;

            var profileResponseTask = _client.SendAsync<ProfilerTakePreciseCoverageResponse>("Profiler.takePreciseCoverage");
            await Task.WhenAll(
               profileResponseTask,
               _client.SendAsync("Profiler.stopPreciseCoverage"),
               _client.SendAsync("Profiler.disable"),
               _client.SendAsync("Debugger.disable")
           );
            _client.MessageReceived -= client_MessageReceived;

            var coverage = new List<CoverageEntry>();
            foreach (var entry in profileResponseTask.Result.Result)
            {

                _scriptURLs.TryGetValue(entry.ScriptId, out var url);
                _scriptSources.TryGetValue(entry.ScriptId, out var text);

                if (text == null || url == null)
                {
                    continue;
                }

                var flattenRanges = new List<ProfilerTakePreciseCoverageResponseRange>();
                foreach (var func in entry.Functions)
                {
                    flattenRanges.Add(func.Ranges);
                }
                var ranges = ConvertToDisjointRanges(flattenRanges);
                coverage.Add(new CoverageEntry
                {
                    Url = url,
                    Ranges = ranges,
                    Text = text
                });
            }
            return coverage.ToArray();
        }

        internal static CoverageEntryRange[] ConvertToDisjointRanges(List<ProfilerTakePreciseCoverageResponseRange> nestedRanges)
        {
            var points = new List<CoverageEntryPoint>();
            foreach (var range in nestedRanges)
            {
                points.Add(new CoverageEntryPoint
                {
                    Offset = range.StartOffset,
                    Type = 0,
                    Range = range
                });

                points.Add(new CoverageEntryPoint
                {
                    Offset = range.EndOffset,
                    Type = 1,
                    Range = range
                });
            }

            points.Sort();

            var hitCountStack = new List<int>();
            var results = new List<CoverageEntryRange>();
            var lastOffset = 0;

            // Run scanning line to intersect all ranges.
            foreach (var point in points)
            {
                if (hitCountStack.Count > 0 && lastOffset < point.Offset && hitCountStack[hitCountStack.Count - 1] > 0)
                {
                    var lastResult = results.Count > 0 ? results[results.Count - 1] : null;
                    if (lastResult != null && lastResult.End == lastOffset)
                    {
                        lastResult.End = point.Offset;
                    }
                    else
                    {
                        results.Add(new CoverageEntryRange
                        {
                            Start = lastOffset,
                            End = point.Offset
                        });
                    }
                }

                lastOffset = point.Offset;
                if (point.Type == 0)
                {
                    hitCountStack.Add(point.Range.Count);
                }
                else
                {
                    hitCountStack.RemoveAt(0);
                }
            }
            // Filter out empty ranges.
            return results.Where(range => range.End - range.Start > 1).ToArray();
        }

        private void client_MessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.MessageID)
            {
                case "Debugger.scriptParsed":
                    OnScriptParsed(e.MessageData.ToObject<DebuggerScriptParsedResponse>());
                    break;
                case "Runtime.executionContextsCleared":
                    OnExecutionContextsCleared();
                    break;
            }
        }

        private async void OnScriptParsed(DebuggerScriptParsedResponse scriptParseResponse)
        {
            if (scriptParseResponse.Url == null)
            {
                return;
            }

            try
            {
                var response = await _client.SendAsync("Debugger.getScriptSource", new { scriptId = scriptParseResponse.ScriptId });
                _scriptURLs.Add(scriptParseResponse.ScriptId, scriptParseResponse.Url);
                _scriptSources.Add(scriptParseResponse.ScriptId, response.scriptSource.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void OnExecutionContextsCleared()
        {
            if (!_resetOnNavigation)
            {
                return;
            }

            _scriptURLs.Clear();
            _scriptSources.Clear();
        }
    }
}