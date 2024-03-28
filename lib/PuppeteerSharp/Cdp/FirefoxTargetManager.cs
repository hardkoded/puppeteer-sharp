#pragma warning disable CS0067 // Temporal, do not merge with this
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
    internal class FirefoxTargetManager : ITargetManager
    {
        private readonly Connection _connection;
        private readonly Func<TargetInfo, CDPSession, CDPSession, CdpTarget> _targetFactoryFunc;
        private readonly Func<CdpTarget, bool> _targetFilterFunc;
        private readonly ILogger<FirefoxTargetManager> _logger;
        private readonly AsyncDictionaryHelper<string, CdpTarget> _availableTargetsByTargetId = new("Target {0} not found");
        private readonly ConcurrentDictionary<string, CdpTarget> _availableTargetsBySessionId = new();
        private readonly ConcurrentDictionary<string, TargetInfo> _discoveredTargetsByTargetId = new();
        private readonly TaskCompletionSource<bool> _initializeCompletionSource = new();
        private List<string> _targetsIdsForInit = [];

        public FirefoxTargetManager(
            Connection connection,
            Func<TargetInfo, CDPSession, CDPSession, CdpTarget> targetFactoryFunc,
            Func<Target, bool> targetFilterFunc)
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

            _targetsIdsForInit = [.. _discoveredTargetsByTargetId.Keys];
            await _initializeCompletionSource.Task.ConfigureAwait(false);
        }

        public AsyncDictionaryHelper<string, CdpTarget> GetAvailableTargets() => _availableTargetsByTargetId;

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
            => e.Session.MessageReceived -= OnMessageReceived;

        private void OnTargetCreated(TargetCreatedResponse e)
        {
            if (!_discoveredTargetsByTargetId.TryAdd(e.TargetInfo.TargetId, e.TargetInfo))
            {
                return;
            }

            if (e.TargetInfo.Type == TargetType.Browser && e.TargetInfo.Attached)
            {
                var browserTarget = _targetFactoryFunc(e.TargetInfo, null, null);
                browserTarget.Initialize();
                _availableTargetsByTargetId.AddItem(e.TargetInfo.TargetId, browserTarget);
                FinishInitializationIfReady(e.TargetInfo.TargetId);
            }

            var target = _targetFactoryFunc(e.TargetInfo, null, null);
            if (_targetFilterFunc != null && !_targetFilterFunc(target))
            {
                FinishInitializationIfReady(e.TargetInfo.TargetId);
                return;
            }

            target.Initialize();
            _availableTargetsByTargetId.AddItem(e.TargetInfo.TargetId, target);
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
            var parentSession = sender as CDPSession;
            var targetInfo = e.TargetInfo;
            var session = _connection.GetSession(e.SessionId) ?? throw new PuppeteerException($"Session {e.SessionId} was not created.");
            _availableTargetsByTargetId.TryGetValue(targetInfo.TargetId, out var target);
            session.MessageReceived += OnMessageReceived;
            session.Target = target;
            _availableTargetsBySessionId.TryAdd(session.Id, target);

            parentSession?.OnSessionReady(session);
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
