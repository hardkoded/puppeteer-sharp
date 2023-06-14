#pragma warning disable CS0067 // Temporal, do not merge with this
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal class FirefoxTargetManager : ITargetManager
    {
        private readonly Connection _connection;
        private readonly Func<TargetInfo, CDPSession, Target> _targetFactoryFunc;
        private readonly Func<TargetInfo, bool> _targetFilterFunc;
        private readonly ILogger<FirefoxTargetManager> _logger;
        private readonly ConcurrentDictionary<ICDPConnection, List<TargetInterceptor>> _targetInterceptors = new();
        private readonly ConcurrentDictionary<string, Target> _availableTargetsByTargetId = new();
        private readonly ConcurrentDictionary<string, Target> _availableTargetsBySessionId = new();
        private readonly ConcurrentDictionary<string, TargetInfo> _discoveredTargetsByTargetId = new();
        private readonly TaskCompletionSource<bool> _initializeCompletionSource = new();
        private readonly List<string> _ignoredTargets = new();
        private List<string> _targetsIdsForInit = new();

        public FirefoxTargetManager(
            Connection connection,
            Func<TargetInfo, CDPSession, Target> targetFactoryFunc,
            Func<TargetInfo, bool> targetFilterFunc)
        {
            _connection = connection;
            _targetFilterFunc = targetFilterFunc;
            _targetFactoryFunc = targetFactoryFunc;
            _logger = _connection.LoggerFactory.CreateLogger<FirefoxTargetManager>();
            _connection.MessageReceived += OnMessageReceived;
            _connection.SessionDetached += Connection_SessionDetached;
        }

        public event EventHandler<TargetChangedArgs> TargetAvailable;

        public event EventHandler<TargetChangedArgs> TargetGone;

        public event EventHandler<TargetChangedArgs> TargetChanged;

        public event EventHandler<TargetChangedArgs> TargetDiscovered;

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

        public async Task InitializeAsync()
        {
            await _connection.SendAsync("Target.setDiscoverTargets", new TargetSetDiscoverTargetsRequest
            {
                Discover = true,
                Filter = new[]
                {
                    new TargetSetDiscoverTargetsRequest.DiscoverFilter(),
                },
            }).ConfigureAwait(false);

            _targetsIdsForInit = new List<string>(_discoveredTargetsByTargetId.Keys);
            await _initializeCompletionSource.Task.ConfigureAwait(false);
        }

        public ConcurrentDictionary<string, Target> GetAvailableTargets() => _availableTargetsByTargetId;

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "Target.attachedToTarget":
                        OnAttachedToTarget(sender, e.MessageData.ToObject<TargetAttachedToTargetResponse>(true));
                        return;
                    case "Target.targetCreated":
                        OnTargetCreated(e.MessageData.ToObject<TargetCreatedResponse>(true));
                        return;

                    case "Target.targetDestroyed":
                        OnTargetDestroyed(e.MessageData.ToObject<TargetDestroyedResponse>(true));
                        return;
                }
            }
            catch (Exception ex)
            {
                var message = $"Browser failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                _connection.Close(message);
            }
        }

        private void Connection_SessionDetached(object sender, SessionEventArgs e)
        {
            e.Session.MessageReceived -= OnMessageReceived;
            _targetInterceptors.TryRemove(e.Session, out var _);
        }

        private void OnTargetCreated(TargetCreatedResponse e)
        {
            if (!_discoveredTargetsByTargetId.TryAdd(e.TargetInfo.TargetId, e.TargetInfo))
            {
                return;
            }

            if (e.TargetInfo.Type == TargetType.Browser && e.TargetInfo.Attached)
            {
                var browserTarget = _targetFactoryFunc(e.TargetInfo, null);
                _availableTargetsByTargetId[e.TargetInfo.TargetId] = browserTarget;
                FinishInitializationIfReady(e.TargetInfo.TargetId);
            }

            if (_targetFilterFunc != null && !_targetFilterFunc(e.TargetInfo))
            {
                _ignoredTargets.Add(e.TargetInfo.TargetId);
                FinishInitializationIfReady(e.TargetInfo.TargetId);
                return;
            }

            var target = _targetFactoryFunc(e.TargetInfo, null);
            _availableTargetsByTargetId[e.TargetInfo.TargetId] = target;
            TargetAvailable?.Invoke(
                this,
                new TargetChangedArgs
                {
                    Target = target,
                    TargetInfo = e.TargetInfo,
                });
            FinishInitializationIfReady(target.TargetId);
        }

        private void OnTargetDestroyed(TargetDestroyedResponse e)
        {
            _discoveredTargetsByTargetId.TryRemove(e.TargetId, out var targetInfo);
            FinishInitializationIfReady(e.TargetId);

            if (_availableTargetsByTargetId.TryRemove(e.TargetId, out var target))
            {
                TargetGone?.Invoke(this, new TargetChangedArgs { Target = target, TargetInfo = targetInfo });
            }
        }

        private void OnAttachedToTarget(object sender, TargetAttachedToTargetResponse e)
        {
            var parent = sender as ICDPConnection;
            var targetInfo = e.TargetInfo;
            var session = _connection.GetSession(e.SessionId) ?? throw new PuppeteerException($"Session {e.SessionId} was not created.");
            var existingTarget = _availableTargetsByTargetId.TryGetValue(targetInfo.TargetId, out var target);
            session.MessageReceived += OnMessageReceived;

            _availableTargetsBySessionId.TryAdd(session.Id, target);

            if (_targetInterceptors.TryGetValue(parent, out var interceptors))
            {
                foreach (var interceptor in interceptors)
                {
                    Target parentTarget = null;
                    if (parent is CDPSession parentSession && !_availableTargetsBySessionId.TryGetValue(parentSession.Id, out parentTarget))
                    {
                        throw new PuppeteerException("Parent session not found in attached targets");
                    }

                    interceptor(target, parentTarget);
                }
            }
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
    }
}
#pragma warning restore CS0067
