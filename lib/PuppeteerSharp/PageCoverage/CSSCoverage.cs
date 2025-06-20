using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using CefSharp.DevTools.Profiler;
using CefSharp.Dom.Helpers;
using CefSharp.Dom.Helpers.Json;
using CefSharp.Dom.Messaging;
using Microsoft.Extensions.Logging;

namespace CefSharp.Dom.PageCoverage
{
    internal class CSSCoverage
    {
        private readonly ConcurrentDictionary<string, (string Url, string Source)> _stylesheets = new();
        private readonly DeferredTaskQueue _callbackQueue = new();
        private readonly ILogger _logger;

        private DevToolsConnection _connection;
        private bool _enabled;
        private bool _resetOnNavigation;

        public CSSCoverage(DevToolsConnection connection)
        {
            _connection = connection;
            _enabled = false;
            _logger = _connection.LoggerFactory.CreateLogger<CSSCoverage>();
            _resetOnNavigation = false;
        }

        internal void UpdateClient(DevToolsConnection connection) => _connection = connection;

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

                if (url.StartsWith("chrome-error://"))
                {
                    continue;
                }

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

        private async void OnConnectionMessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "CSS.styleSheetAdded":
                        await _callbackQueue.Enqueue(()
                            => OnStyleSheetAddedAsync(e.MessageData.ToObject<CSSStyleSheetAddedResponse>())).ConfigureAwait(false);
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
