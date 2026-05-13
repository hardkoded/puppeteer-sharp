using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp
{
    internal class ChromeTargetManager : ITargetManager
    {
        private readonly List<string> _ignoredTargets = new();
        private readonly Connection _connection;
        private readonly Func<TargetInfo, CDPSession, CDPSession, CdpTarget> _targetFactoryFunc;
        private readonly Func<Target, bool> _targetFilterFunc;
        private readonly ILogger<ChromeTargetManager> _logger;
        private readonly AsyncDictionaryHelper<string, CdpTarget> _attachedTargetsByTargetId = new("Target {0} not found");
        private readonly ConcurrentDictionary<string, CdpTarget> _attachedTargetsBySessionId = new();
        private readonly ConcurrentDictionary<string, TargetInfo> _discoveredTargetsByTargetId = new();
        private readonly TaskCompletionSource<bool> _initializeCompletionSource = new();

        // IDs of tab targets detected while running the initial Target.setAutoAttach
        // request. These are the targets whose initialization we want to await for
        // before resolving puppeteer.connect() or launch() to avoid flakiness.
        // Whenever a sub-target whose parent is a tab target is attached, we remove
        // the tab target from this list. Once the list is empty, we resolve the
        // initializeDeferred.
        private readonly ConcurrentSet<string> _targetsIdsForInit = [];

        private readonly string[] _blockList;
        private readonly string[] _allowList;

        // This is false until the connection-level Target.setAutoAttach request is
        // done. It indicates whether we are running the initial auto-attach step or
        // if we are handling targets after that.
        private bool _initialAttachDone;

        public ChromeTargetManager(
            Connection connection,
            Func<TargetInfo, CDPSession, CDPSession, CdpTarget> targetFactoryFunc,
            Func<Target, bool> targetFilterFunc,
            string[] blockList = null,
            string[] allowList = null)
        {
            if (blockList != null && allowList != null)
            {
                throw new PuppeteerException("Cannot specify both blocklist and allowlist");
            }

            ValidateUrlPatterns(blockList);
            ValidateUrlPatterns(allowList);

            _connection = connection;
            _targetFilterFunc = targetFilterFunc;
            _targetFactoryFunc = targetFactoryFunc;
            _blockList = blockList;
            _allowList = allowList;
            _logger = _connection.LoggerFactory.CreateLogger<ChromeTargetManager>();
            _connection.MessageReceived += OnMessageReceived;
            _connection.SessionDetached += Connection_SessionDetached;
        }

        public event EventHandler<TargetChangedArgs> TargetAvailable;

        public event EventHandler<TargetChangedArgs> TargetGone;

        public event EventHandler<TargetChangedArgs> TargetChanged;

        public event EventHandler<TargetChangedArgs> TargetDiscovered;

        public AsyncDictionaryHelper<string, CdpTarget> GetAvailableTargets() => _attachedTargetsByTargetId;

        public async Task InitializeAsync()
        {
            await _connection.SendAsync("Target.setDiscoverTargets", new TargetSetDiscoverTargetsRequest
            {
                Discover = true,
                Filter =
                [
                    new TargetSetDiscoverTargetsRequest.DiscoverFilter() { Type = "tab", Exclude = true, },
                    new TargetSetDiscoverTargetsRequest.DiscoverFilter()
                ],
            }).ConfigureAwait(false);

            // Exclude page targets from connection-level auto-attach so we attach to the
            // owning tab first. The tab session then auto-attaches to its page child via
            // the session-level setAutoAttach below, establishing the browser -> tab ->
            // page session hierarchy. Page._tabId depends on this hierarchy (and Chrome's
            // Extensions.triggerAction rejects targetIds that are not tab targets).
            await _connection.SendAsync(
                "Target.setAutoAttach",
                new TargetSetAutoAttachRequest()
                {
                    WaitForDebuggerOnStart = true,
                    Flatten = true,
                    AutoAttach = true,
                    Filter = new[]
                    {
                        new TargetSetDiscoverTargetsRequest.DiscoverFilter() { Type = "page", Exclude = true, },
                        new TargetSetDiscoverTargetsRequest.DiscoverFilter(),
                    },
                }).ConfigureAwait(false);

            _initialAttachDone = true;
            FinishInitializationIfReady();

            await _initializeCompletionSource.Task.ConfigureAwait(false);
        }

        public IEnumerable<ITarget> GetChildTargets(ITarget target) => target.ChildTargets;

        public bool IsUrlAllowed(string url)
        {
            var hasBlockList = _blockList != null && _blockList.Length > 0;
            var hasAllowList = _allowList != null && _allowList.Length > 0;

            if (!hasBlockList && !hasAllowList)
            {
                return true;
            }

            // Always allow internal or setup pages
            if (string.IsNullOrEmpty(url) || url == "about:blank")
            {
                return true;
            }

            if (hasBlockList)
            {
                foreach (var pattern in _blockList)
                {
                    if (MatchesUrlPattern(url, pattern))
                    {
                        return false;
                    }
                }
            }

            if (hasAllowList)
            {
                foreach (var pattern in _allowList)
                {
                    if (MatchesUrlPattern(url, pattern))
                    {
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        private static void ValidateUrlPatterns(string[] patterns)
        {
            if (patterns == null)
            {
                return;
            }

            foreach (var pattern in patterns)
            {
                // Basic validation to catch common URLPattern syntax errors (e.g., unbalanced parentheses).
                // .NET has no native URLPattern API, so this only checks a subset of URLPattern invariants.
                // The URLPattern spec rejects patterns with unbalanced parentheses.
                var depth = 0;
                foreach (var c in pattern)
                {
                    if (c == '(')
                    {
                        depth++;
                    }
                    else if (c == ')')
                    {
                        depth--;
                        if (depth < 0)
                        {
                            throw new PuppeteerException($"Invalid URLPattern: '{pattern}'. Unbalanced parentheses.");
                        }
                    }
                }

                if (depth != 0)
                {
                    throw new PuppeteerException($"Invalid URLPattern: '{pattern}'. Unbalanced parentheses.");
                }
            }
        }

        private static bool MatchesUrlPattern(string url, string pattern)
        {
            // Convert URLPattern glob syntax to a regex pattern.
            // Supported wildcards: * matches any sequence of characters (within segment),
            // and the leading *:// matches any scheme.
            // This is a simplified implementation covering the common cases used in tests.
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
            return Regex.IsMatch(url, regexPattern, RegexOptions.IgnoreCase);
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "Target.attachedToTarget":
                        _ = OnAttachedToTargetHandlingExceptionsAsync(sender, e.MessageID, e.MessageData.ToObject<TargetAttachedToTargetResponse>());
                        return;

                    case "Target.detachedFromTarget":
                        OnDetachedFromTarget(sender, e.MessageData.ToObject<TargetDetachedFromTargetResponse>());
                        return;

                    case "Target.targetCreated":
                        OnTargetCreated(e.MessageData.ToObject<TargetCreatedResponse>());
                        return;

                    case "Target.targetDestroyed":
                        _ = OnTargetDestroyedAsync(e.MessageID, e.MessageData.ToObject<TargetDestroyedResponse>());
                        return;

                    case "Target.targetInfoChanged":
                        OnTargetInfoChanged(e.MessageData.ToObject<TargetCreatedResponse>());
                        return;
                }
            }
            catch (Exception ex)
            {
                HandleExceptionOnMessageReceived(e.MessageID, ex);
            }
        }

        private void Connection_SessionDetached(object sender, SessionEventArgs e)
        {
            e.Session.MessageReceived -= OnMessageReceived;
        }

        private void OnTargetCreated(TargetCreatedResponse e)
        {
            _discoveredTargetsByTargetId[e.TargetInfo.TargetId] = e.TargetInfo;

            TargetDiscovered?.Invoke(this, new TargetChangedArgs { TargetInfo = e.TargetInfo });

            if (e.TargetInfo.Type == TargetType.Browser && e.TargetInfo.Attached)
            {
                if (_attachedTargetsByTargetId.ContainsKey(e.TargetInfo.TargetId))
                {
                    return;
                }

                var target = _targetFactoryFunc(e.TargetInfo, null, null);
                target.Initialize();
                _attachedTargetsByTargetId.AddItem(e.TargetInfo.TargetId, target);
            }
        }

        private async Task OnTargetDestroyedAsync(string messageId, TargetDestroyedResponse e)
        {
            try
            {
                _discoveredTargetsByTargetId.TryRemove(e.TargetId, out var targetInfo);
                FinishInitializationIfReady(e.TargetId);

                if (targetInfo?.Type == TargetType.ServiceWorker)
                {
                    // Special case for service workers: report TargetGone event when
                    // the worker is destroyed.
                    if (_attachedTargetsByTargetId.TryRemove(e.TargetId, out var target))
                    {
                        TargetGone?.Invoke(this, new TargetChangedArgs { Target = target, TargetInfo = targetInfo });
                    }
                }
            }
            catch (Exception ex)
            {
                HandleExceptionOnMessageReceived(messageId, ex);
            }
        }

        private void OnTargetInfoChanged(TargetCreatedResponse e)
        {
            _discoveredTargetsByTargetId[e.TargetInfo.TargetId] = e.TargetInfo;

            if (_ignoredTargets.Contains(e.TargetInfo.TargetId) ||
                !_attachedTargetsByTargetId.TryGetValue(e.TargetInfo.TargetId, out var target) ||
                !e.TargetInfo.Attached)
            {
                return;
            }

            var previousURL = target.Url;
            var wasInitialized = target.IsInitialized;

            if (IsPageTargetBecomingPrimary(target, e.TargetInfo))
            {
                var session = target.Session;
                session.ParentSession?.OnSwapped(session);
            }

            target.TargetInfoChanged(e.TargetInfo);

            if (wasInitialized && previousURL != target.Url)
            {
                TargetChanged?.Invoke(this, new TargetChangedArgs
                {
                    Target = target,
                    TargetInfo = e.TargetInfo,
                });
            }
        }

        private bool IsPageTargetBecomingPrimary(CdpTarget target, TargetInfo newTargetInfo)
            => !string.IsNullOrEmpty(target.TargetInfo.Subtype) && string.IsNullOrEmpty(newTargetInfo.Subtype);

        private async Task SilentDetachAsync(CDPSession session, ICDPConnection parentConnection)
        {
            try
            {
                await session.SendAsync("Runtime.runIfWaitingForDebugger").ConfigureAwait(false);

                // We don't use session.Detach() because that dispatches all commands on
                // the connection instead of the parent session.
                await parentConnection.SendAsync(
                    "Target.detachFromTarget",
                    new TargetDetachFromTargetRequest
                    {
                        SessionId = session.Id,
                    }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "silentDetach failed.");
            }
        }

        private CdpTarget GetParentTarget(ICDPConnection parentConnection)
            => parentConnection is CdpCDPSession parentSession ? parentSession.Target as CdpTarget : null;

        private async Task OnAttachedToTargetAsync(object sender, TargetAttachedToTargetResponse e)
        {
            var parentConnection = sender as ICDPConnection;
            var parentSession = sender as CDPSession;

            var targetInfo = e.TargetInfo;
            var session = _connection.GetSession(e.SessionId) ?? throw new PuppeteerException($"Session {e.SessionId} was not created.");

            if (!_connection.IsAutoAttached(targetInfo.TargetId))
            {
                return;
            }

            // If we connect to a browser that is already open,
            // immediately detach from any tab that is on the blocklist.
            if (!_initialAttachDone && !IsUrlAllowed(targetInfo.Url))
            {
                await SilentDetachAsync(session, parentConnection).ConfigureAwait(false);
                return;
            }

            if (targetInfo.Type == TargetType.ServiceWorker)
            {
                await SilentDetachAsync(session, parentConnection).ConfigureAwait(false);
                if (_attachedTargetsByTargetId.ContainsKey(targetInfo.TargetId))
                {
                    return;
                }

                var workerTarget = _targetFactoryFunc(targetInfo, null, null);
                workerTarget.Initialize();
                _attachedTargetsByTargetId.AddItem(targetInfo.TargetId, workerTarget);
                TargetAvailable?.Invoke(this, new TargetChangedArgs { Target = workerTarget });
                return;
            }

            var isExistingTarget = _attachedTargetsByTargetId.TryGetValue(targetInfo.TargetId, out var target);
            if (!isExistingTarget)
            {
                target = _targetFactoryFunc(targetInfo, session, parentSession);
            }

            var parentTarget = GetParentTarget(parentConnection);

            if (_targetFilterFunc?.Invoke(target) == false)
            {
                _ignoredTargets.Add(targetInfo.TargetId);
                if (parentTarget?.TargetInfo.Type == TargetType.Tab)
                {
                    FinishInitializationIfReady(parentTarget.TargetId);
                }

                await SilentDetachAsync(session, parentConnection).ConfigureAwait(false);
                return;
            }

            if (targetInfo.Type == TargetType.Tab && !_initialAttachDone)
            {
                _targetsIdsForInit.Add(targetInfo.TargetId);
            }

            session.MessageReceived += OnMessageReceived;

            if (isExistingTarget)
            {
                session.Target = target;
                _attachedTargetsBySessionId.TryAdd(session.Id, target);
            }
            else
            {
                target.Initialize();

                _attachedTargetsByTargetId.AddItem(targetInfo.TargetId, target);
                _attachedTargetsBySessionId.TryAdd(session.Id, target);
            }

            parentTarget?.AddChildTarget(target);
            (parentSession ?? parentConnection as CDPSession)?.OnSessionReady(session);

            if (!isExistingTarget)
            {
                TargetAvailable?.Invoke(this, new TargetChangedArgs { Target = target });
            }

            if (parentTarget?.TargetInfo.Type == TargetType.Tab)
            {
                FinishInitializationIfReady(parentTarget.TargetId);
            }

            try
            {
                await Task.WhenAll(
                    session.SendAsync("Target.setAutoAttach", new TargetSetAutoAttachRequest
                    {
                        WaitForDebuggerOnStart = true,
                        Flatten = true,
                        AutoAttach = true,
                    }),
                    MaybeSetupNetworkBlockListAsync(session),
                    session.SendAsync("Runtime.runIfWaitingForDebugger")).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to call setAutoAttach and runIfWaitingForDebugger");
            }
        }

        private async Task OnAttachedToTargetHandlingExceptionsAsync(object sender, string messageId, TargetAttachedToTargetResponse e)
        {
            try
            {
                await OnAttachedToTargetAsync(sender, e).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                HandleExceptionOnMessageReceived(messageId, ex);
            }
        }

        private void HandleExceptionOnMessageReceived(string messageId, Exception ex)
        {
            var message = $"Browser failed to process {messageId}. {ex.Message}. {ex.StackTrace}";
            _logger.LogError(ex, message);
            _connection.Close(message);
        }

        private void FinishInitializationIfReady(string targetId = null)
        {
            if (targetId != null)
            {
                _targetsIdsForInit.Remove(targetId);
            }

            // If we are still initializing it might be that we have not learned about
            // some targets yet.
            if (!_initialAttachDone)
            {
                return;
            }

            if (_targetsIdsForInit.Count == 0)
            {
                _initializeCompletionSource.TrySetResult(true);
            }
        }

        private void OnDetachedFromTarget(object sender, TargetDetachedFromTargetResponse e)
        {
            if (!_attachedTargetsBySessionId.TryRemove(e.SessionId, out var target))
            {
                return;
            }

            if (sender is CdpCDPSession parentSession)
            {
                parentSession.Target.RemoveChildTarget(target);
            }

            _attachedTargetsByTargetId.TryRemove(target.TargetId, out _);
            TargetGone?.Invoke(this, new TargetChangedArgs { Target = target });
        }

        private async Task MaybeSetupNetworkBlockListAsync(CDPSession session)
        {
            var hasBlockList = _blockList != null && _blockList.Length > 0;
            var hasAllowList = _allowList != null && _allowList.Length > 0;

            if (!hasBlockList && !hasAllowList)
            {
                return;
            }

            var matchedNetworkConditions = new List<MatchedNetworkCondition>();

            if (hasBlockList)
            {
                foreach (var pattern in _blockList)
                {
                    matchedNetworkConditions.Add(new MatchedNetworkCondition
                    {
                        UrlPattern = pattern,
                        Offline = true,
                        Latency = 0,
                        DownloadThroughput = -1,
                        UploadThroughput = -1,
                    });
                }
            }

            if (hasAllowList)
            {
                foreach (var pattern in _allowList)
                {
                    matchedNetworkConditions.Add(new MatchedNetworkCondition
                    {
                        UrlPattern = pattern,
                        Offline = false,
                        Latency = 0,
                        DownloadThroughput = -1,
                        UploadThroughput = -1,
                    });
                }

                // Block everything else
                matchedNetworkConditions.Add(new MatchedNetworkCondition
                {
                    UrlPattern = string.Empty,
                    Offline = true,
                    Latency = 0,
                    DownloadThroughput = -1,
                    UploadThroughput = -1,
                });
            }

            await session.SendAsync(
                "Network.emulateNetworkConditionsByRule",
                new NetworkEmulateNetworkConditionsByRuleRequest
                {
                    // 'Offline' at the request level is for legacy blocklist compatibility (deprecated in Chrome 149).
                    // Only set it when using a blocklist; allowlist mode uses per-rule offline flags instead.
                    Offline = hasBlockList ? true : null,
                    MatchedNetworkConditions = matchedNetworkConditions.ToArray(),
                }).ConfigureAwait(false);
        }
    }
}
