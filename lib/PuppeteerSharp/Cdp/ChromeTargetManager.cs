using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private readonly ConcurrentSet<string> _targetsIdsForInit = [];
        private readonly TaskCompletionSource<bool> _initializeCompletionSource = new();
        private readonly Browser _browser;

        // Needed for .NET only to prevent race conditions between StoreExistingTargetsForInit and OnAttachedToTarget
        private readonly int _targetDiscoveryTimeout;
        private readonly TaskCompletionSource<bool> _targetDiscoveryCompletionSource = new();

        public ChromeTargetManager(
            Connection connection,
            Func<TargetInfo, CDPSession, CDPSession, CdpTarget> targetFactoryFunc,
            Func<Target, bool> targetFilterFunc,
            Browser browser,
            int targetDiscoveryTimeout = 0)
        {
            _connection = connection;
            _targetFilterFunc = targetFilterFunc;
            _targetFactoryFunc = targetFactoryFunc;
            _logger = _connection.LoggerFactory.CreateLogger<ChromeTargetManager>();
            _connection.MessageReceived += OnMessageReceived;
            _connection.SessionDetached += Connection_SessionDetached;
            _targetDiscoveryTimeout = targetDiscoveryTimeout;
            _browser = browser;
        }

        public event EventHandler<TargetChangedArgs> TargetAvailable;

        public event EventHandler<TargetChangedArgs> TargetGone;

        public event EventHandler<TargetChangedArgs> TargetChanged;

        public event EventHandler<TargetChangedArgs> TargetDiscovered;

        public AsyncDictionaryHelper<string, CdpTarget> GetAvailableTargets() => _attachedTargetsByTargetId;

        public async Task InitializeAsync()
        {
            try
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
            }
            finally
            {
                _targetDiscoveryCompletionSource.SetResult(true);
            }

            StoreExistingTargetsForInit();

            await _connection.SendAsync(
                "Target.setAutoAttach",
                new TargetSetAutoAttachRequest()
                {
                    WaitForDebuggerOnStart = true,
                    Flatten = true,
                    AutoAttach = true,
                }).ConfigureAwait(false);

            FinishInitializationIfReady();

            await _initializeCompletionSource.Task.ConfigureAwait(false);
        }

        private void StoreExistingTargetsForInit()
        {
            foreach (var kv in _discoveredTargetsByTargetId)
            {
                var targetForFilter = new CdpTarget(
                    kv.Value,
                    null,
                    null,
                    this,
                    null,
                    _browser.ScreenshotTaskQueue);

                if ((_targetFilterFunc == null || _targetFilterFunc(targetForFilter)) &&
                    kv.Value.Type != TargetType.Browser)
                {
                    _targetsIdsForInit.Add(kv.Key);
                }
            }
        }

        private async Task EnsureTargetsIdsForInitAsync()
        {
            if (_targetDiscoveryTimeout > 0)
            {
                await _targetDiscoveryCompletionSource.Task.WithTimeout(_targetDiscoveryTimeout).ConfigureAwait(false);
            }
            else
            {
                await _targetDiscoveryCompletionSource.Task.ConfigureAwait(false);
            }
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "Target.attachedToTarget":
                        _ = OnAttachedToTargetHandlingExceptionsAsync(sender, e.MessageID, e.MessageData.ToObject<TargetAttachedToTargetResponse>(true));
                        return;

                    case "Target.detachedFromTarget":
                        OnDetachedFromTarget(sender, e.MessageData.ToObject<TargetDetachedFromTargetResponse>(true));
                        return;

                    case "Target.targetCreated":
                        OnTargetCreated(e.MessageData.ToObject<TargetCreatedResponse>(true));
                        return;

                    case "Target.targetDestroyed":
                        _ = OnTargetDestroyedAsync(e.MessageID, e.MessageData.ToObject<TargetDestroyedResponse>(true));
                        return;

                    case "Target.targetInfoChanged":
                        OnTargetInfoChanged(e.MessageData.ToObject<TargetCreatedResponse>(true));
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
                await EnsureTargetsIdsForInitAsync().ConfigureAwait(false);
                FinishInitializationIfReady(e.TargetId);

                if (targetInfo?.Type == TargetType.ServiceWorker && _attachedTargetsByTargetId.TryRemove(e.TargetId, out var target))
                {
                    TargetGone?.Invoke(this, new TargetChangedArgs { Target = target, TargetInfo = targetInfo });
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

        private bool IsPageTargetBecomingPrimary(Target target, TargetInfo newTargetInfo)
            => !string.IsNullOrEmpty(target.TargetInfo.Subtype) && string.IsNullOrEmpty(newTargetInfo.Subtype);

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

            if (targetInfo.Type == TargetType.ServiceWorker)
            {
                await EnsureTargetsIdsForInitAsync().ConfigureAwait(false);
                FinishInitializationIfReady(targetInfo.TargetId);
                await SilentDetach().ConfigureAwait(false);
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

            if (_targetFilterFunc?.Invoke(target) == false)
            {
                _ignoredTargets.Add(targetInfo.TargetId);
                await EnsureTargetsIdsForInitAsync().ConfigureAwait(false);
                FinishInitializationIfReady(targetInfo.TargetId);
                await SilentDetach().ConfigureAwait(false);
                return;
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

            (parentSession ?? parentConnection as CDPSession)?.OnSessionReady(session);

            await EnsureTargetsIdsForInitAsync().ConfigureAwait(false);
            _targetsIdsForInit.Remove(target.TargetId);

            if (!isExistingTarget)
            {
                TargetAvailable?.Invoke(this, new TargetChangedArgs { Target = target });
            }

            FinishInitializationIfReady();

            try
            {
                await Task.WhenAll(
                    session.SendAsync("Target.setAutoAttach", new TargetSetAutoAttachRequest
                    {
                        WaitForDebuggerOnStart = true,
                        Flatten = true,
                        AutoAttach = true,
                    }),
                    session.SendAsync("Runtime.runIfWaitingForDebugger")).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to call setAutoAttach and runIfWaitingForDebugger");
            }

            return;

            async Task SilentDetach()
            {
                try
                {
                    await session.SendAsync("Runtime.runIfWaitingForDebugger").ConfigureAwait(false);
                    await parentConnection!.SendAsync(
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

            _attachedTargetsByTargetId.TryRemove(target.TargetId, out _);
            TargetGone?.Invoke(this, new TargetChangedArgs { Target = target });
        }
    }
}
