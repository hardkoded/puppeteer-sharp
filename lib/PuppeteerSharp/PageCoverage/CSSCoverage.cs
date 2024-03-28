using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.PageCoverage
{
    internal class CSSCoverage
    {
        private readonly ConcurrentDictionary<string, (string Url, string Source)> _stylesheets = new();
        private readonly DeferredTaskQueue _callbackQueue = new();
        private readonly ILogger _logger;

        private CDPSession _client;
        private bool _enabled;
        private bool _resetOnNavigation;

        public CSSCoverage(CDPSession client)
        {
            _client = client;
            _enabled = false;
            _logger = _client.Connection.LoggerFactory.CreateLogger<CSSCoverage>();
            _resetOnNavigation = false;
        }

        internal void UpdateClient(CDPSession client) => _client = client;

        internal Task StartAsync(CoverageStartOptions options)
        {
            if (_enabled)
            {
                throw new InvalidOperationException("CSSCoverage is already enabled");
            }

            _resetOnNavigation = options.ResetOnNavigation;
            _enabled = true;
            _stylesheets.Clear();

            _client.MessageReceived += Client_MessageReceived;

            return Task.WhenAll(
                _client.SendAsync("DOM.enable"),
                _client.SendAsync("CSS.enable"),
                _client.SendAsync("CSS.startRuleUsageTracking"));
        }

        internal async Task<CoverageEntry[]> StopAsync()
        {
            if (!_enabled)
            {
                throw new InvalidOperationException("CSSCoverage is not enabled");
            }

            _enabled = false;

            var trackingResponse = await _client.SendAsync<CSSStopRuleUsageTrackingResponse>("CSS.stopRuleUsageTracking").ConfigureAwait(false);

            // Wait until we've stopped CSS tracking before stopping listening for messages and finishing up, so that
            // any pending OnStyleSheetAddedAsync tasks can collect the remaining style sheet coverage.
            _client.MessageReceived -= Client_MessageReceived;
            await _callbackQueue.DrainAsync().ConfigureAwait(false);

            await Task.WhenAll(
                _client.SendAsync("CSS.disable"),
                _client.SendAsync("DOM.disable")).ConfigureAwait(false);

            var styleSheetIdToCoverage = new Dictionary<string, List<CoverageRange>>();
            foreach (var entry in trackingResponse.RuleUsage)
            {
                styleSheetIdToCoverage.TryGetValue(entry.StyleSheetId, out var ranges);
                if (ranges == null)
                {
                    ranges = new List<CoverageRange>();
                    styleSheetIdToCoverage[entry.StyleSheetId] = ranges;
                }

                ranges.Add(new CoverageRange
                {
                    StartOffset = entry.StartOffset,
                    EndOffset = entry.EndOffset,
                    Count = entry.Used ? 1 : 0,
                });
            }

            var coverage = new List<CoverageEntry>();
            foreach (var kv in _stylesheets.ToArray())
            {
                var styleSheetId = kv.Key;
                var url = kv.Value.Url;
                var text = kv.Value.Source;
                styleSheetIdToCoverage.TryGetValue(styleSheetId, out var responseRanges);
                var ranges = Coverage.ConvertToDisjointRanges(responseRanges ?? new List<CoverageRange>());
                coverage.Add(new CoverageEntry
                {
                    Url = url,
                    Ranges = ranges,
                    Text = text,
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
                        await _callbackQueue.Enqueue(() => OnStyleSheetAddedAsync(e.MessageData.ToObject<CSSStyleSheetAddedResponse>(true))).ConfigureAwait(false);
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
                    StyleSheetId = styleSheetAddedResponse.Header.StyleSheetId,
                }).ConfigureAwait(false);

                _stylesheets.TryAdd(styleSheetAddedResponse.Header.StyleSheetId, (styleSheetAddedResponse.Header.SourceURL, response.Text));
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

            _stylesheets.Clear();
        }
    }
}
