using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom.Helpers;
using CefSharp.DevTools.Dom.Helpers.Json;
using CefSharp.DevTools.Dom.Messaging;
using Microsoft.Extensions.Logging;

namespace CefSharp.DevTools.Dom.PageCoverage
{
    internal class CSSCoverage
    {
        private readonly DevToolsConnection _connection;
        private readonly ConcurrentDictionary<string, (string Url, string Source)> _stylesheets;
        private readonly DeferredTaskQueue _callbackQueue;
        private readonly ILogger _logger;

        private bool _enabled;
        private bool _resetOnNavigation;

        public CSSCoverage(DevToolsConnection connection)
        {
            _connection = connection;
            _enabled = false;
            _stylesheets = new ConcurrentDictionary<string, (string Url, string Source)>();
            _logger = _connection.LoggerFactory.CreateLogger<CSSCoverage>();
            _callbackQueue = new DeferredTaskQueue();

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
            _stylesheets.Clear();

            _connection.MessageReceived += OnConnectionMessageReceived;

            return Task.WhenAll(
                _connection.SendAsync("DOM.enable"),
                _connection.SendAsync("CSS.enable"),
                _connection.SendAsync("CSS.startRuleUsageTracking"));
        }

        internal async Task<CoverageEntry[]> StopAsync()
        {
            if (!_enabled)
            {
                throw new InvalidOperationException("CSSCoverage is not enabled");
            }
            _enabled = false;

            var trackingResponse = await _connection.SendAsync<CSSStopRuleUsageTrackingResponse>("CSS.stopRuleUsageTracking").ConfigureAwait(false);

            // Wait until we've stopped CSS tracking before stopping listening for messages and finishing up, so that
            // any pending OnStyleSheetAddedAsync tasks can collect the remaining style sheet coverage.
            _connection.MessageReceived -= OnConnectionMessageReceived;
            await _callbackQueue.DrainAsync().ConfigureAwait(false);

            await Task.WhenAll(
                _connection.SendAsync("CSS.disable"),
                _connection.SendAsync("DOM.disable")).ConfigureAwait(false);

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
            foreach (var kv in _stylesheets.ToArray())
            {
                var styleSheetId = kv.Key;
                var url = kv.Value.Url;
                var text = kv.Value.Source;
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

        private async void OnConnectionMessageReceived(object sender, MessageEventArgs e)
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
                _connection.Close(message);
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
                var response = await _connection.SendAsync<CssGetStyleSheetTextResponse>("CSS.getStyleSheetText", new CssGetStyleSheetTextRequest
                {
                    StyleSheetId = styleSheetAddedResponse.Header.StyleSheetId
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
