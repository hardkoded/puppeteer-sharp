using Microsoft.Extensions.Logging;
using PuppeteerSharp.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp.PageCoverage
{
    internal class CSSCoverage
    {
        private readonly CDPSession _client;
        private readonly Dictionary<string, string> _stylesheetURLs;
        private readonly Dictionary<string, string> _stylesheetSources;
        private readonly ILogger _logger;

        private bool _enabled;
        private bool _resetOnNavigation;

        public CSSCoverage(CDPSession client)
        {
            _client = client;
            _enabled = false;
            _stylesheetURLs = new Dictionary<string, string>();
            _stylesheetSources = new Dictionary<string, string>();
            _logger = _client.Connection.LoggerFactory.CreateLogger<CSSCoverage>();

            _resetOnNavigation = false;
        }

        internal async Task StartAsync(CoverageStartOptions options)
        {
            if (_enabled)
            {
                throw new InvalidOperationException("CSSCoverage is already enabled");
            }

            _resetOnNavigation = options.ResetOnNavigation;
            _enabled = true;
            _stylesheetURLs.Clear();
            _stylesheetSources.Clear();

            _client.MessageReceived += client_MessageReceived;

            await Task.WhenAll(
                _client.SendAsync("DOM.enable"),
                _client.SendAsync("CSS.enable"),
                _client.SendAsync("CSS.startRuleUsageTracking")
            );
        }

        internal async Task<CoverageEntry[]> StopAsync()
        {
            if (!_enabled)
            {
                throw new InvalidOperationException("CSSCoverage is not enabled");
            }
            _enabled = false;

            var ruleTrackingResponseTask = _client.SendAsync<CSSStopRuleUsageTrackingResponse>("CSS.stopRuleUsageTracking");
            await Task.WhenAll(
                ruleTrackingResponseTask,
                _client.SendAsync("CSS.disable"),
                _client.SendAsync("DOM.disable")
           );
            _client.MessageReceived -= client_MessageReceived;

            var styleSheetIdToCoverage = new Dictionary<string, List<CoverageResponseRange>>();
            foreach (var entry in ruleTrackingResponseTask.Result.RuleUsage)
            {
                styleSheetIdToCoverage.TryGetValue(entry.StyleSheetId, out var ranges);
                if (ranges == null)
                {
                    ranges = new List<CoverageResponseRange>();
                    styleSheetIdToCoverage[entry.StyleSheetId] = ranges;
                }
                ranges.Add(new CoverageResponseRange
                {
                    StartOffset = entry.StartOffset,
                    EndOffset = entry.EndOffset,
                    Count = entry.Used ? 1 : 0,
                });
            }

            var coverage = new List<CoverageEntry>();
            foreach (var styleSheetId in _stylesheetURLs.Keys)
            {
                var url = _stylesheetURLs[styleSheetId];
                var text = _stylesheetSources[styleSheetId];
                styleSheetIdToCoverage.TryGetValue(styleSheetId, out var responseRanges);
                var ranges = Coverage.ConvertToDisjointRanges(responseRanges ?? new List<CoverageResponseRange>());
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
                case "CSS.styleSheetAdded":
                    await OnStyleSheetAdded(e.MessageData.ToObject<CSSStyleSheetAddedResponse>());
                    break;
                case "Runtime.executionContextsCleared":
                    OnExecutionContextsCleared();
                    break;
            }
        }

        private async Task OnStyleSheetAdded(CSSStyleSheetAddedResponse styleSheetAddedResponse)
        {
            if (string.IsNullOrEmpty(styleSheetAddedResponse.Header.SourceURL))
            {
                return;
            }

            try
            {
                var response = await _client.SendAsync("CSS.getStyleSheetText", new
                {
                    styleSheetId = styleSheetAddedResponse.Header.StyleSheetId
                });

                _stylesheetURLs.Add(styleSheetAddedResponse.Header.StyleSheetId, styleSheetAddedResponse.Header.SourceURL);
                _stylesheetSources.Add(styleSheetAddedResponse.Header.StyleSheetId, response.text.ToString());
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

            _stylesheetURLs.Clear();
            _stylesheetSources.Clear();
        }
    }
}