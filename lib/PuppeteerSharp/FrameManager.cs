using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal class FrameManager
    {
        private const string RefererHeaderName = "referer";
        private const string UtilityWorldName = "__puppeteer_utility_world__";

        private readonly ConcurrentDictionary<string, ExecutionContext> _contextIdToContext;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, Frame> _frames;
        private readonly AsyncDictionaryHelper<string, Frame> _asyncFrames;
        private readonly List<string> _isolatedWorlds = new();
        private readonly TaskQueue _eventsQueue = new();
        private bool _ensureNewDocumentNavigation;

        internal FrameManager(CDPSession client, Page page, bool ignoreHTTPSErrors, TimeoutSettings timeoutSettings)
        {
            Client = client;
            Page = page;
            _frames = new ConcurrentDictionary<string, Frame>();
            _contextIdToContext = new ConcurrentDictionary<string, ExecutionContext>();
            _logger = Client.Connection.LoggerFactory.CreateLogger<FrameManager>();
            NetworkManager = new NetworkManager(client, ignoreHTTPSErrors, this);
            TimeoutSettings = timeoutSettings;
            _asyncFrames = new AsyncDictionaryHelper<string, Frame>(_frames, "Frame {0} not found");

            Client.MessageReceived += Client_MessageReceived;
        }

        internal event EventHandler<FrameEventArgs> FrameAttached;

        internal event EventHandler<FrameEventArgs> FrameDetached;

        internal event EventHandler<FrameEventArgs> FrameSwapped;

        internal event EventHandler<FrameEventArgs> FrameNavigated;

        internal event EventHandler<FrameEventArgs> FrameNavigatedWithinDocument;

        internal event EventHandler<FrameEventArgs> LifecycleEvent;

        internal CDPSession Client { get; }

        internal NetworkManager NetworkManager { get; }

        internal Frame MainFrame { get; set; }

        internal Page Page { get; }

        internal TimeoutSettings TimeoutSettings { get; }

        internal async Task InitializeAsync(CDPSession client = null)
        {
            client ??= Client;
            var getFrameTreeTask = client.SendAsync<PageGetFrameTreeResponse>("Page.getFrameTree");
            var autoAttachTask = client != Client
                ? client.SendAsync("Target.setAutoAttach", new TargetSetAutoAttachRequest
                {
                    AutoAttach = true,
                    WaitForDebuggerOnStart = false,
                    Flatten = true,
                })
                : Task.CompletedTask;

            await Task.WhenAll(
                client.SendAsync("Page.enable"),
                getFrameTreeTask,
                autoAttachTask).ConfigureAwait(false);

            await HandleFrameTreeAsync(client, new FrameTree(getFrameTreeTask.Result.FrameTree)).ConfigureAwait(false);

            await Task.WhenAll(
                client.SendAsync("Page.setLifecycleEventsEnabled", new PageSetLifecycleEventsEnabledRequest { Enabled = true }),
                client.SendAsync("Runtime.enable"),
                client == Client ? NetworkManager.InitializeAsync() : Task.CompletedTask).ConfigureAwait(false);

            await EnsureIsolatedWorldAsync(client, UtilityWorldName).ConfigureAwait(false);
        }

        internal ExecutionContext ExecutionContextById(int contextId, CDPSession session = null)
        {
            session ??= Client;
            var key = $"{session.Id}:{contextId}";
            _contextIdToContext.TryGetValue(key, out var context);

            if (context == null)
            {
                _logger.LogError("INTERNAL ERROR: missing context with id = {ContextId}", contextId);
            }
            return context;
        }

        public async Task<Response> NavigateFrameAsync(Frame frame, string url, NavigationOptions options)
        {
            var referrer = string.IsNullOrEmpty(options.Referer)
               ? NetworkManager.ExtraHTTPHeaders?.GetValueOrDefault(RefererHeaderName)
               : options.Referer;
            var timeout = options?.Timeout ?? TimeoutSettings.NavigationTimeout;

            using (var watcher = new LifecycleWatcher(this, frame, options?.WaitUntil, timeout))
            {
                try
                {
                    var navigateTask = NavigateAsync(Client, url, referrer, frame.Id);
                    var task = await Task.WhenAny(
                        watcher.TimeoutOrTerminationTask,
                        navigateTask).ConfigureAwait(false);

                    await task.ConfigureAwait(false);

                    task = await Task.WhenAny(
                        watcher.TimeoutOrTerminationTask,
                        _ensureNewDocumentNavigation ? watcher.NewDocumentNavigationTask : watcher.SameDocumentNavigationTask).ConfigureAwait(false);

                    await task.ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new NavigationException(ex.Message, ex);
                }

                return watcher.NavigationResponse;
            }
        }

        private async Task NavigateAsync(CDPSession client, string url, string referrer, string frameId)
        {
            var response = await client.SendAsync<PageNavigateResponse>("Page.navigate", new PageNavigateRequest
            {
                Url = url,
                Referrer = referrer ?? string.Empty,
                FrameId = frameId
            }).ConfigureAwait(false);

            _ensureNewDocumentNavigation = !string.IsNullOrEmpty(response.LoaderId);

            if (!string.IsNullOrEmpty(response.ErrorText))
            {
                throw new NavigationException(response.ErrorText, url);
            }
        }

        public async Task<Response> WaitForFrameNavigationAsync(Frame frame, NavigationOptions options = null)
        {
            var timeout = options?.Timeout ?? TimeoutSettings.NavigationTimeout;
            using (var watcher = new LifecycleWatcher(this, frame, options?.WaitUntil, timeout))
            {
                var raceTask = await Task.WhenAny(
                    watcher.NewDocumentNavigationTask,
                    watcher.SameDocumentNavigationTask,
                    watcher.TimeoutOrTerminationTask).ConfigureAwait(false);

                await raceTask.ConfigureAwait(false);

                return watcher.NavigationResponse;
            }
        }

        private void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            _ = _eventsQueue.Enqueue(async () =>
            {
                try
                {
                    switch (e.MessageID)
                    {
                        case "Page.frameAttached":
                            OnFrameAttached(sender as CDPSession, e.MessageData.ToObject<PageFrameAttachedResponse>());
                            break;

                        case "Page.frameNavigated":
                            await OnFrameNavigatedAsync(e.MessageData.ToObject<PageFrameNavigatedResponse>(true).Frame).ConfigureAwait(false);
                            break;

                        case "Page.navigatedWithinDocument":
                            OnFrameNavigatedWithinDocument(e.MessageData.ToObject<NavigatedWithinDocumentResponse>(true));
                            break;

                        case "Page.frameDetached":
                            OnFrameDetached(e.MessageData.ToObject<PageFrameDetachedResponse>(true));
                            break;

                        case "Page.frameStartedLoading":
                            OnFrameStartedLoading(e.MessageData.ToObject<BasicFrameResponse>(true));
                            break;

                        case "Page.frameStoppedLoading":
                            OnFrameStoppedLoading(e.MessageData.ToObject<BasicFrameResponse>(true));
                            break;

                        case "Runtime.executionContextCreated":
                            await OnExecutionContextCreatedAsync(e.MessageData.ToObject<RuntimeExecutionContextCreatedResponse>(true).Context, sender as CDPSession).ConfigureAwait(false);
                            break;

                        case "Runtime.executionContextDestroyed":
                            OnExecutionContextDestroyed(e.MessageData.ToObject<RuntimeExecutionContextDestroyedResponse>(true).ExecutionContextId, sender as CDPSession);
                            break;
                        case "Runtime.executionContextsCleared":
                            OnExecutionContextsCleared(sender as CDPSession);
                            break;
                        case "Page.lifecycleEvent":
                            OnLifeCycleEvent(e.MessageData.ToObject<LifecycleEventResponse>(true));
                            break;
                        case "Target.attachedToTarget":
                            await OnAttachedToTargetAsync(e.MessageData.ToObject<TargetAttachedToTargetResponse>(true)).ConfigureAwait(false);
                            break;
                        case "Target.detachedFromTarget":
                            OnDetachedFromTarget(e.MessageData.ToObject<TargetDetachedFromTargetResponse>(true));
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var message = $"Connection failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
                    _logger.LogError(ex, message);
                    Client.Close(message);
                }
            });
        }

        private void OnDetachedFromTarget(TargetDetachedFromTargetResponse e)
        {
            _frames.TryGetValue(e.TargetId, out var frame);
            if (frame != null && frame.IsOopFrame)
            {
              RemoveFramesRecursively(frame);
            }
        }

        private async Task OnAttachedToTargetAsync(TargetAttachedToTargetResponse e)
        {
            if (e.TargetInfo.Type != TargetType.IFrame)
            {
                return;
            }

            _frames.TryGetValue(e.TargetInfo.TargetId, out var frame);
            var session = Connection.FromSession(Client).GetSession(e.SessionId);
            if (frame != null)
            {
                frame.UpdateClient(session);
            }

            session.MessageReceived += Client_MessageReceived;
            await InitializeAsync(session).ConfigureAwait(false);
        }

        private void OnFrameStartedLoading(BasicFrameResponse e)
        {
            if (_frames.TryGetValue(e.FrameId, out var frame))
            {
                frame.OnLoadingStarted();
            }
        }

        private void OnFrameStoppedLoading(BasicFrameResponse e)
        {
            if (_frames.TryGetValue(e.FrameId, out var frame))
            {
                frame.OnLoadingStopped();
                LifecycleEvent?.Invoke(this, new FrameEventArgs(frame));
            }
        }

        private void OnLifeCycleEvent(LifecycleEventResponse e)
        {
            if (_frames.TryGetValue(e.FrameId, out var frame))
            {
                frame.OnLifecycleEvent(e.LoaderId, e.Name);
                LifecycleEvent?.Invoke(this, new FrameEventArgs(frame));
            }
        }

        private void OnExecutionContextsCleared(CDPSession session)
        {
            foreach (var key in _contextIdToContext.Keys.ToArray())
            {
                var context = _contextIdToContext[key];
                if (context.Client != session)
                {
                    continue;
                }
                if (context.World != null)
                {
                    context.World.SetContext(null);
                }
                _contextIdToContext.TryRemove(key, out var _);
            }
        }

        private void OnExecutionContextDestroyed(int contextId, CDPSession session)
        {
            var key = $"{session.Id}:{contextId}";
            if (_contextIdToContext.TryRemove(key, out var context))
            {
                if (context.World != null)
                {
                    context.World.SetContext(null);
                }
            }
        }

        private async Task OnExecutionContextCreatedAsync(ContextPayload contextPayload, CDPSession session)
        {
            var frameId = contextPayload.AuxData?.FrameId;
            var frame = !string.IsNullOrEmpty(frameId) ? await GetFrameAsync(frameId).ConfigureAwait(false) : null;
            DOMWorld world = null;

            if (frame != null)
            {
                if (frame.Client != session)
                {
                    return;
                }

                if (contextPayload.AuxData?.IsDefault == true)
                {
                    world = frame.MainWorld;
                }
                else if (contextPayload.Name == UtilityWorldName && !frame.SecondaryWorld.HasContext)
                {
                    // In case of multiple sessions to the same target, there's a race between
                    // connections so we might end up creating multiple isolated worlds.
                    // We can use either.
                    world = frame.SecondaryWorld;
                }
            }

            var context = new ExecutionContext(frame?.Client ?? Client, contextPayload, world);
            if (world != null)
            {
                world.SetContext(context);
            }
            var key = $"{session.Id}:{contextPayload.Id}";
            _contextIdToContext[key] = context;
        }

        private void OnFrameDetached(PageFrameDetachedResponse e)
        {
            if (_frames.TryGetValue(e.FrameId, out var frame))
            {
                if (e.Reason == FrameDetachedReason.Remove)
                {
                    RemoveFramesRecursively(frame);
                }
                else if (e.Reason == FrameDetachedReason.Swap)
                {
                    FrameSwapped?.Invoke(frame, new FrameEventArgs(frame));
                }
            }
        }

        private async Task OnFrameNavigatedAsync(FramePayload framePayload)
        {
            var isMainFrame = string.IsNullOrEmpty(framePayload.ParentId);
            var frame = isMainFrame ? MainFrame : await GetFrameAsync(framePayload.Id).ConfigureAwait(false);

            Contract.Assert(isMainFrame || frame != null, "We either navigate top level or have old version of the navigated frame");

            // Detach all child frames first.
            if (frame != null)
            {
                while (frame.ChildFrames.Count > 0)
                {
                    RemoveFramesRecursively(frame.ChildFrames[0]);
                }
            }

            // Update or create main frame.
            if (isMainFrame)
            {
                if (frame != null)
                {
                    // Update frame id to retain frame identity on cross-process navigation.
                    if (frame.Id != null)
                    {
                        _frames.TryRemove(frame.Id, out _);
                    }
                    frame.Id = framePayload.Id;
                }
                else
                {
                    // Initial main frame navigation.
                    frame = new Frame(this, null, framePayload.Id, Client);
                }
                _asyncFrames.AddItem(framePayload.Id, frame);
                MainFrame = frame;
            }

            // Update frame payload.
            frame.Navigated(framePayload);

            FrameNavigated?.Invoke(this, new FrameEventArgs(frame));
        }

        internal Frame[] GetFrames() => _frames.Values.ToArray();

        private void OnFrameNavigatedWithinDocument(NavigatedWithinDocumentResponse e)
        {
            if (_frames.TryGetValue(e.FrameId, out var frame))
            {
                frame.NavigatedWithinDocument(e.Url);

                var eventArgs = new FrameEventArgs(frame);
                FrameNavigatedWithinDocument?.Invoke(this, eventArgs);
                FrameNavigated?.Invoke(this, eventArgs);
            }
        }

        private void RemoveFramesRecursively(Frame frame)
        {
            while (frame.ChildFrames.Count > 0)
            {
                RemoveFramesRecursively(frame.ChildFrames[0]);
            }
            frame.Detach();
            _frames.TryRemove(frame.Id, out _);
            FrameDetached?.Invoke(this, new FrameEventArgs(frame));
        }

        private void OnFrameAttached(CDPSession session, PageFrameAttachedResponse frameAttached)
            => OnFrameAttached(session, frameAttached.FrameId, frameAttached.ParentFrameId);

        private void OnFrameAttached(CDPSession session, string frameId, string parentFrameId)
        {
            if (_frames.TryGetValue(frameId, out var existingFrame))
            {
                if (session != null && existingFrame.IsOopFrame)
                {
                    existingFrame.UpdateClient(session);
                }
                return;
            }

            if (!_frames.ContainsKey(frameId) && _frames.ContainsKey(parentFrameId))
            {
                var parentFrame = _frames[parentFrameId];
                var frame = new Frame(this, parentFrame, frameId, session);
                _frames[frame.Id] = frame;
                FrameAttached?.Invoke(this, new FrameEventArgs(frame));
            }
        }

        private async Task HandleFrameTreeAsync(CDPSession session, FrameTree frameTree)
        {
            if (!string.IsNullOrEmpty(frameTree.Frame.ParentId))
            {
                OnFrameAttached(session, frameTree.Frame.Id, frameTree.Frame.ParentId);
            }

            await OnFrameNavigatedAsync(frameTree.Frame).ConfigureAwait(false);

            if (frameTree.Childs != null)
            {
                foreach (var child in frameTree.Childs)
                {
                    await HandleFrameTreeAsync(session, child).ConfigureAwait(false);
                }
            }
        }

        private async Task EnsureIsolatedWorldAsync(CDPSession session, string name)
        {
            var key = $"{session.Id}:{name}";
            if (_isolatedWorlds.Contains(key))
            {
                return;
            }
            _isolatedWorlds.Add(key);
            await session.SendAsync("Page.addScriptToEvaluateOnNewDocument", new PageAddScriptToEvaluateOnNewDocumentRequest
            {
                Source = $"//# sourceURL={ExecutionContext.EvaluationScriptUrl}",
                WorldName = name,
            }).ConfigureAwait(false);

            try
            {
                await Task.WhenAll(GetFrames()
                    .Where(frame => frame.Client == session)
                    .Select(frame => session.SendAsync("Page.createIsolatedWorld", new PageCreateIsolatedWorldRequest
                    {
                        FrameId = frame.Id,
                        GrantUniveralAccess = true,
                        WorldName = name
                    }))).ConfigureAwait(false);
            }
            catch (PuppeteerException ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        internal Task<Frame> GetFrameAsync(string frameId) => _asyncFrames.GetItemAsync(frameId);

        internal Task<Frame> TryGetFrameAsync(string frameId) => _asyncFrames.TryGetItemAsync(frameId);
    }
}
