using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom.Helpers.Json;
using CefSharp.DevTools.Dom.Messaging;
using Microsoft.Extensions.Logging;

namespace CefSharp.DevTools.Dom.PageCoverage
{
    internal class JSCoverage
    {
        private readonly DevToolsConnection _connection;
        private readonly Dictionary<string, string> _scriptURLs;
        private readonly Dictionary<string, string> _scriptSources;
        private readonly ILogger _logger;

        private bool _enabled;
        private bool _resetOnNavigation;
        private bool _reportAnonymousScripts;

        public JSCoverage(DevToolsConnection connection)
        {
            _connection = connection;
            _enabled = false;
            _scriptURLs = new Dictionary<string, string>();
            _scriptSources = new Dictionary<string, string>();
            _logger = _connection.LoggerFactory.CreateLogger<JSCoverage>();

            _resetOnNavigation = false;
        }

        internal Task StartAsync(CoverageStartOptions options)
        {
            if (_enabled)
            {
                throw new InvalidOperationException("JSCoverage is already enabled");
            }

            _resetOnNavigation = options.ResetOnNavigation;
            _reportAnonymousScripts = options.ReportAnonymousScripts;
            _enabled = true;
            _scriptURLs.Clear();
            _scriptSources.Clear();

            _connection.MessageReceived += Client_MessageReceived;

            return Task.WhenAll(
                _connection.SendAsync("Profiler.enable"),
                _connection.SendAsync("Profiler.startPreciseCoverage", new ProfilerStartPreciseCoverageRequest
                {
                    CallCount = false,
                    Detailed = true
                }),
                _connection.SendAsync("Debugger.enable"),
                _connection.SendAsync("Debugger.setSkipAllPauses", new DebuggerSetSkipAllPausesRequest { Skip = true }));
        }

        internal async Task<CoverageEntry[]> StopAsync()
        {
            if (!_enabled)
            {
                throw new InvalidOperationException("JSCoverage is not enabled");
            }
            _enabled = false;

            var profileResponseTask = _connection.SendAsync<ProfilerTakePreciseCoverageResponse>("Profiler.takePreciseCoverage");
            await Task.WhenAll(
               profileResponseTask,
               _connection.SendAsync("Profiler.stopPreciseCoverage"),
               _connection.SendAsync("Profiler.disable"),
               _connection.SendAsync("Debugger.disable")).ConfigureAwait(false);
            _connection.MessageReceived -= Client_MessageReceived;

            var coverage = new List<CoverageEntry>();
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
                coverage.Add(new CoverageEntry
                {
                    Url = url,
                    Ranges = ranges,
                    Text = text
                });
            }
            return coverage.ToArray();
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
                _connection.Close(message);
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
                var response = await _connection.SendAsync<DebuggerGetScriptSourceResponse>("Debugger.getScriptSource", new DebuggerGetScriptSourceRequest
                {
                    ScriptId = scriptParseResponse.ScriptId
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
