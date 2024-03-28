using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.PageCoverage
{
    internal class JSCoverage
    {
        private readonly Dictionary<string, string> _scriptURLs = new();
        private readonly Dictionary<string, string> _scriptSources = new();
        private readonly ILogger _logger;

        private CDPSession _client;
        private bool _enabled;
        private bool _resetOnNavigation;
        private bool _reportAnonymousScripts;
        private bool _includeRawScriptCoverage;

        public JSCoverage(CDPSession client)
        {
            _client = client;
            _enabled = false;
            _logger = _client.Connection.LoggerFactory.CreateLogger<JSCoverage>();

            _resetOnNavigation = false;
        }

        internal void UpdateClient(CDPSession client) => _client = client;

        internal Task StartAsync(CoverageStartOptions options)
        {
            if (_enabled)
            {
                throw new InvalidOperationException("JSCoverage is already enabled");
            }

            _resetOnNavigation = options.ResetOnNavigation;
            _reportAnonymousScripts = options.ReportAnonymousScripts;
            _includeRawScriptCoverage = options.IncludeRawScriptCoverage;
            _enabled = true;
            _scriptURLs.Clear();
            _scriptSources.Clear();

            _client.MessageReceived += Client_MessageReceived;

            return Task.WhenAll(
                _client.SendAsync("Profiler.enable"),
                _client.SendAsync("Profiler.startPreciseCoverage", new ProfilerStartPreciseCoverageRequest
                {
                    CallCount = _includeRawScriptCoverage,
                    Detailed = true,
                }),
                _client.SendAsync("Debugger.enable"),
                _client.SendAsync("Debugger.setSkipAllPauses", new DebuggerSetSkipAllPausesRequest { Skip = true }));
        }

        internal async Task<JSCoverageEntry[]> StopAsync()
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
               _client.SendAsync("Debugger.disable")).ConfigureAwait(false);
            _client.MessageReceived -= Client_MessageReceived;

            var coverage = new List<JSCoverageEntry>();
            foreach (var entry in profileResponseTask.Result.Result)
            {
                _scriptURLs.TryGetValue(entry.ScriptId, out var url);
                if (string.IsNullOrEmpty(url) && _reportAnonymousScripts)
                {
                    url = "debugger://VM" + entry.ScriptId;
                }

                if (string.IsNullOrEmpty(url) ||
                    !_scriptSources.TryGetValue(entry.ScriptId, out var text))
                {
                    continue;
                }

                var flattenRanges = entry.Functions.SelectMany(f => f.Ranges).ToList();
                var ranges = Coverage.ConvertToDisjointRanges(flattenRanges);
                coverage.Add(new JSCoverageEntry
                {
                    Url = url,
                    Ranges = ranges,
                    Text = text,
                    RawScriptCoverage = _includeRawScriptCoverage ? entry : null,
                });
            }

            return [.. coverage];
        }

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "Debugger.scriptParsed":
                        await OnScriptParsedAsync(e.MessageData.ToObject<DebuggerScriptParsedResponse>(true)).ConfigureAwait(false);
                        break;
                    case "Runtime.executionContextsCleared":
                        OnExecutionContextsCleared();
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = $"JSCoverage failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                _client.Close(message);
            }
        }

        private async Task OnScriptParsedAsync(DebuggerScriptParsedResponse scriptParseResponse)
        {
            if (scriptParseResponse.Url == ExecutionContext.EvaluationScriptUrl ||
                (string.IsNullOrEmpty(scriptParseResponse.Url) && !_reportAnonymousScripts))
            {
                return;
            }

            try
            {
                var response = await _client.SendAsync<DebuggerGetScriptSourceResponse>("Debugger.getScriptSource", new DebuggerGetScriptSourceRequest
                {
                    ScriptId = scriptParseResponse.ScriptId,
                }).ConfigureAwait(false);
                _scriptURLs.Add(scriptParseResponse.ScriptId, scriptParseResponse.Url);
                _scriptSources.Add(scriptParseResponse.ScriptId, response.ScriptSource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
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
