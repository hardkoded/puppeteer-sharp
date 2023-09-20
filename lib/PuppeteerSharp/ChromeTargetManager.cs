using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal class ChromeTargetManager : ITargetManager
    {
        private readonly List<string> _ignoredTargets = new();
        private readonly Connection _connection;
        private readonly Func<TargetInfo, CDPSession, Target> _targetFactoryFunc;
        private readonly Func<Target, bool> _targetFilterFunc;
        private readonly ILogger<ChromeTargetManager> _logger;
        private readonly AsyncDictionaryHelper<string, Target> _attachedTargetsByTargetId = new("Target {0} not found");
        private readonly ConcurrentDictionary<string, Target> _attachedTargetsBySessionId = new();
        private readonly ConcurrentDictionary<string, TargetInfo> _discoveredTargetsByTargetId = new();
        private readonly ConcurrentDictionary<ICDPConnection, List<TargetInterceptor>> _targetInterceptors = new();
        private readonly List<string> _targetsIdsForInit = new();
        private readonly TaskCompletionSource<bool> _initializeCompletionSource = new();

        // Needed for .NET only to prevent race conditions between StoreExistingTargetsForInit and OnAttachedToTarget
        private readonly int _targetDiscoveryTimeout;
        private readonly TaskCompletionSource<bool> _targetDiscoveryCompletionSource = new();

        public ChromeTargetManager(
            Connection connection,
            Func<TargetInfo, CDPSession, Target> targetFactoryFunc,
            Func<Target, bool> targetFilterFunc,
            int targetDiscoveryTimeout = 0)
        {
            _connection = connection;
            _targetFilterFunc = targetFilterFunc;
            _targetFactoryFunc = targetFactoryFunc;
            _logger = _connection.LoggerFactory.CreateLogger<ChromeTargetManager>();
            _connection.MessageReceived += OnMessageReceived;
            _connection.SessionDetached += Connection_SessionDetached;
            _targetDiscoveryTimeout = targetDiscoveryTimeout;

            _ = _connection.SendAsync("Target.setDiscoverTargets", new TargetSetDiscoverTargetsRequest
            {
                Discover = true,
                Filter = new[]
                {
                    new TargetSetDiscoverTargetsRequest.DiscoverFilter()
                    {
                        Type = "tab",
                        Exclude = true,
                    },
                    new TargetSetDiscoverTargetsRequest.DiscoverFilter(),
                },
            }).ContinueWith(
                t =>
                {
                    try
                    {
                        if (t.IsFaulted)
                        {
                            _logger.LogError(t.Exception, "Target.setDiscoverTargets failed");
                        }
                        else
                        {
                            StoreExistingTargetsForInit();
                        }
                    }
                    finally
                    {
                        _targetDiscoveryCompletionSource.SetResult(true);
                    }
                },
                TaskScheduler.Default);
        }

        public event EventHandler<TargetChangedArgs> TargetAvailable;

        public event EventHandler<TargetChangedArgs> TargetGone;

        public event EventHandler<TargetChangedArgs> TargetChanged;

        public event EventHandler<TargetChangedArgs> TargetDiscovered;

        public AsyncDictionaryHelper<string, Target> GetAvailableTargets() => _attachedTargetsByTargetId;

        public async Task InitializeAsync()
        {
            await _connection.SendAsync("Target.setAutoAttach", new TargetSetAutoAttachRequest()
            {
                WaitForDebuggerOnStart = true,
                Flatten = true,
                AutoAttach = true,
            }).ConfigureAwait(false);

            await _targetDiscoveryCompletionSource.Task.ConfigureAwait(false);
            FinishInitializationIfReady();

            await _initializeCompletionSource.Task.ConfigureAwait(false);
        }

        public void AddTargetInterceptor(CDPSession session, TargetInterceptor interceptor)
        {
            var interceptors = _targetInterceptors.GetOrAdd(session, static _ => new());
            interceptors.Add(interceptor);
        }

        public void RemoveTargetInterceptor(CDPSession session, TargetInterceptor interceptor)
        {
            _targetInterceptors.TryGetValue(session, out var interceptors);
            interceptors?.Remove(interceptor);
        }

        private void StoreExistingTargetsForInit()
        {
            foreach (var kv in _discoveredTargetsByTargetId)
            {
                var targetForFilter = new Target(
                    kv.Value,
                    null,
                    null,
                    this,
                    null);

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
            _targetInterceptors.TryRemove(e.Session, out var _);
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

                var target = _targetFactoryFunc(e.TargetInfo, null);
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

        private async Task OnAttachedToTargetAsync(object sender, TargetAttachedToTargetResponse e)
        {
            var parent = sender as ICDPConnection;
            var targetInfo = e.TargetInfo;
            var session = _connection.GetSession(e.SessionId) ?? throw new PuppeteerException($"Session {e.SessionId} was not created.");

            async Task SilentDetach()
            {
                try
                {
                    await session.SendAsync("Runtime.runIfWaitingForDebugger").ConfigureAwait(false);
                    await parent.SendAsync(
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

            if (!_connection.IsAutoAttached(targetInfo.TargetId))
            {
                return;
            }

            if (targetInfo.Type == TargetType.ServiceWorker &&
                _connection.IsAutoAttached(targetInfo.TargetId))
            {
                await EnsureTargetsIdsForInitAsync().ConfigureAwait(false);
                FinishInitializationIfReady(targetInfo.TargetId);
                await SilentDetach().ConfigureAwait(false);
                if (_attachedTargetsByTargetId.ContainsKey(targetInfo.TargetId))
                {
                    return;
                }

                var workerTarget = _targetFactoryFunc(targetInfo, null);
                workerTarget.Initialize();
                _attachedTargetsByTargetId.AddItem(targetInfo.TargetId, workerTarget);
                TargetAvailable?.Invoke(this, new TargetChangedArgs { Target = workerTarget });
                return;
            }

            var existingTarget = _attachedTargetsByTargetId.TryGetValue(targetInfo.TargetId, out var target);
            if (!existingTarget)
            {
                target = _targetFactoryFunc(targetInfo, session);
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

            if (existingTarget)
            {
                _attachedTargetsBySessionId.TryAdd(session.Id, target);
            }
            else
            {
                target.Initialize();

                _attachedTargetsByTargetId.AddItem(targetInfo.TargetId, target);
                _attachedTargetsBySessionId.TryAdd(session.Id, target);
            }

            if (_targetInterceptors.TryGetValue(parent, out var interceptors))
            {
                foreach (var interceptor in interceptors)
                {
                    Target parentTarget = null;
                    if (parent is CDPSession parentSession && !_attachedTargetsBySessionId.TryGetValue(parentSession.Id, out parentTarget))
                    {
                        throw new PuppeteerException("Parent session not found in attached targets");
                    }

                    interceptor(target, parentTarget);
                }
            }

            await EnsureTargetsIdsForInitAsync().ConfigureAwait(false);
            _targetsIdsForInit.Remove(target.TargetId);

            if (!existingTarget)
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
                _logger.LogError("Failed to call setAutoAttach and runIfWaitingForDebugger", ex);
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
