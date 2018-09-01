using PuppeteerSharp.Messaging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PuppeteerSharp.PageCoverage
{
    internal class JSCoverage
    {
        private readonly CDPSession _client;
        private readonly Dictionary<string, string> _scriptURLs;
        private readonly Dictionary<string, string> _scriptSources;
        private readonly ILogger _logger;

        private bool _enabled;
        private bool _resetOnNavigation;
        private bool _reportAnonymousScripts;

        public JSCoverage(CDPSession client)
        {
            _client = client;
            _enabled = false;
            _scriptURLs = new Dictionary<string, string>();
            _scriptSources = new Dictionary<string, string>();
            _logger = _client.Connection.LoggerFactory.CreateLogger<JSCoverage>();

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

            _client.MessageReceived += client_MessageReceived;

            return Task.WhenAll(
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
           ).ConfigureAwait(false);
            _client.MessageReceived -= client_MessageReceived;

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

        private async void client_MessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.MessageID)
            {
                case "Debugger.scriptParsed":
                    await OnScriptParsed(e.MessageData.ToObject<DebuggerScriptParsedResponse>()).ConfigureAwait(false);
                    break;
                case "Runtime.executionContextsCleared":
                    OnExecutionContextsCleared();
                    break;
            }
        }

        private async Task OnScriptParsed(DebuggerScriptParsedResponse scriptParseResponse)
        {
            if (scriptParseResponse.Url == ExecutionContext.EvaluationScriptUrl ||
                (string.IsNullOrEmpty(scriptParseResponse.Url) && !_reportAnonymousScripts))
            {
                return;
            }

            try
            {
                var response = await _client.SendAsync("Debugger.getScriptSource", new { scriptId = scriptParseResponse.ScriptId }).ConfigureAwait(false);
                _scriptURLs.Add(scriptParseResponse.ScriptId, scriptParseResponse.Url);
                _scriptSources.Add(scriptParseResponse.ScriptId, response.scriptSource.ToString());
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