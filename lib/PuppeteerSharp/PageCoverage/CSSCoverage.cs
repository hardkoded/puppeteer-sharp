using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Messaging;

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

        internal Task StartAsync(CoverageStartOptions options)
        {
            if (_enabled)
            {
                throw new InvalidOperationException("CSSCoverage is already enabled");
            }

            _resetOnNavigation = options.ResetOnNavigation;
            _enabled = true;
            _stylesheetURLs.Clear();
            _stylesheetSources.Clear();

            _client.MessageReceived += Client_MessageReceived;

            return Task.WhenAll(
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

            var trackingResponse = await _client.SendAsync<CSSStopRuleUsageTrackingResponse>("CSS.stopRuleUsageTracking").ConfigureAwait(false);
            await Task.WhenAll(
                _client.SendAsync("CSS.disable"),
                _client.SendAsync("DOM.disable")
            ).ConfigureAwait(false);
            _client.MessageReceived -= Client_MessageReceived;

            var styleSheetIdToCoverage = new Dictionary<string, List<CoverageResponseRange>>();
            foreach (var entry in trackingResponse.RuleUsage)
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

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "CSS.styleSheetAdded":
                        await OnStyleSheetAddedAsync(e.MessageData.ToObject<CSSStyleSheetAddedResponse>(true)).ConfigureAwait(false);
                        break;
                    case "Runtime.executionContextsCleared":
                        OnExecutionContextsCleared();
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = $"CSSCoverage failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                _client.Close(message);
            }
        }

        private async Task OnStyleSheetAddedAsync(CSSStyleSheetAddedResponse styleSheetAddedResponse)
        {
            if (string.IsNullOrEmpty(styleSheetAddedResponse.Header.SourceURL))
            {
                return;
            }

            try
            {
                var response = await _client.SendAsync<CssGetStyleSheetTextResponse>("CSS.getStyleSheetText", new CssGetStyleSheetTextRequest
                {
                    StyleSheetId = styleSheetAddedResponse.Header.StyleSheetId
                }).ConfigureAwait(false);

                _stylesheetURLs.Add(styleSheetAddedResponse.Header.StyleSheetId, styleSheetAddedResponse.Header.SourceURL);
                _stylesheetSources.Add(styleSheetAddedResponse.Header.StyleSheetId, response.Text);
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