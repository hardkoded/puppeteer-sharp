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
        private readonly List<string> _isolatedWorlds = new();
        private readonly List<string> _frameNavigatedReceived = new();
        private readonly TaskQueue _eventsQueue = new();
        private bool _ensureNewDocumentNavigation;

        internal FrameManager(CDPSession client, Page page, bool ignoreHTTPSErrors, TimeoutSettings timeoutSettings)
        {
            Client = client;
            Page = page;
            _contextIdToContext = new ConcurrentDictionary<string, ExecutionContext>();
            _logger = Client.Connection.LoggerFactory.CreateLogger<FrameManager>();
            NetworkManager = new NetworkManager(client, ignoreHTTPSErrors, this);
            TimeoutSettings = timeoutSettings;

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

        internal FrameTree FrameTree { get; private set; } = new();

        public async Task<IResponse> NavigateFrameAsync(Frame frame, string url, NavigationOptions options)
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

        public async Task<IResponse> WaitForFrameNavigationAsync(Frame frame, NavigationOptions options = null)
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

        internal async Task InitializeAsync(CDPSession client = null)
        {
            try
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

                await HandleFrameTreeAsync(client, getFrameTreeTask.Result.FrameTree).ConfigureAwait(false);

                await Task.WhenAll(
                    client.SendAsync("Page.setLifecycleEventsEnabled", new PageSetLifecycleEventsEnabledRequest { Enabled = true }),
                    client.SendAsync("Runtime.enable"),
                    client == Client ? NetworkManager.InitializeAsync() : Task.CompletedTask).ConfigureAwait(false);

                await EnsureIsolatedWorldAsync(client, UtilityWorldName).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // The target might have been closed before the initialization finished.
                if (
                  ex.Message.Contains("Target closed") ||
                  ex.Message.Contains("Session closed"))
                {
                    return;
                }

                throw;
            }
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

        internal void OnAttachedToTarget(TargetChangedArgs e)
        {
            if (e.TargetInfo.Type != TargetType.IFrame)
            {
                return;
            }

            var frame = GetFrame(e.TargetInfo.TargetId);
            frame?.UpdateClient(e.Target.Session);

            e.Target.Session.MessageReceived += Client_MessageReceived;
            _ = InitializeAsync(e.Target.Session);
        }

        internal Frame GetFrame(string frameid) => FrameTree.GetById(frameid);

        internal Frame[] GetFrames() => FrameTree.Frames;

        private async Task NavigateAsync(CDPSession client, string url, string referrer, string frameId)
        {
            var response = await client.SendAsync<PageNavigateResponse>("Page.navigate", new PageNavigateRequest
            {
                Url = url,
                Referrer = referrer ?? string.Empty,
                FrameId = frameId,
            }).ConfigureAwait(false);

            _ensureNewDocumentNavigation = !string.IsNullOrEmpty(response.LoaderId);

            if (!string.IsNullOrEmpty(response.ErrorText))
            {
                throw new NavigationException(response.ErrorText, url);
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

        private void OnFrameStartedLoading(BasicFrameResponse e)
        {
            var frame = GetFrame(e.FrameId);
            frame?.OnLoadingStarted();
        }

        private void OnFrameStoppedLoading(BasicFrameResponse e)
        {
            var frame = GetFrame(e.FrameId);
            if (frame != null)
            {
                frame.OnLoadingStopped();
                LifecycleEvent?.Invoke(this, new FrameEventArgs(frame));
            }
        }

        private void OnLifeCycleEvent(LifecycleEventResponse e)
        {
            var frame = GetFrame(e.FrameId);
            if (frame != null)
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

                context.World?.ClearContext();

                _contextIdToContext.TryRemove(key, out var _);
            }
        }

        private void OnExecutionContextDestroyed(int contextId, CDPSession session)
        {
            var key = $"{session.Id}:{contextId}";
            if (_contextIdToContext.TryRemove(key, out var context))
            {
                context.World?.ClearContext();
            }
        }

        private async Task OnExecutionContextCreatedAsync(ContextPayload contextPayload, CDPSession session)
        {
            var frameId = contextPayload.AuxData?.FrameId;
            var frame = !string.IsNullOrEmpty(frameId) ? await FrameTree.GetFrameAsync(frameId).ConfigureAwait(false) : null;
            IsolatedWorld world = null;

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
                else if (contextPayload.Name == UtilityWorldName && !frame.PuppeteerWorld.HasContext)
                {
                    // In case of multiple sessions to the same target, there's a race between
                    // connections so we might end up creating multiple isolated worlds.
                    // We can use either.
                    world = frame.PuppeteerWorld;
                }
            }

            var context = new ExecutionContext(frame?.Client ?? Client, contextPayload, world);
            world?.SetContext(context);

            var key = $"{session.Id}:{contextPayload.Id}";
            _contextIdToContext[key] = context;
        }

        private void OnFrameDetached(PageFrameDetachedResponse e)
        {
            var frame = GetFrame(e.FrameId);
            if (frame != null)
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
            // This is in the event handler upstream.
            // It's more consistent having this here.
            _frameNavigatedReceived.Add(framePayload.Id);

            var isMainFrame = string.IsNullOrEmpty(framePayload.ParentId);
            var frame = isMainFrame ? MainFrame : await FrameTree.GetFrameAsync(framePayload.Id).ConfigureAwait(false);

            Contract.Assert(isMainFrame || frame != null, "We either navigate top level or have old version of the navigated frame");

            // Detach all child frames first.
            if (frame != null)
            {
                while (frame.ChildFrames.Any())
                {
                    RemoveFramesRecursively(frame.ChildFrames.First() as Frame);
                }
            }

            // Update or create main frame.
            if (isMainFrame)
            {
                if (frame != null)
                {
                    FrameTree.RemoveFrame(frame);
                    frame.Id = framePayload.Id;
                }
                else
                {
                    // Initial main frame navigation.
                    frame = new Frame(this, null, framePayload.Id, Client);
                }

                MainFrame = frame;
            }

            // Update frame payload.
            frame.Navigated(framePayload);

            FrameNavigated?.Invoke(this, new FrameEventArgs(frame));
        }

        private void OnFrameNavigatedWithinDocument(NavigatedWithinDocumentResponse e)
        {
            var frame = GetFrame(e.FrameId);
            if (frame != null)
            {
                frame.NavigatedWithinDocument(e.Url);

                var eventArgs = new FrameEventArgs(frame);
                FrameNavigatedWithinDocument?.Invoke(this, eventArgs);
                FrameNavigated?.Invoke(this, eventArgs);
            }
        }

        private void RemoveFramesRecursively(Frame frame)
        {
            while (frame.ChildFrames.Any())
            {
                RemoveFramesRecursively(frame.ChildFrames.First() as Frame);
            }

            frame.Detach();
            FrameTree.RemoveFrame(frame);
            FrameDetached?.Invoke(this, new FrameEventArgs(frame));
        }

        private void OnFrameAttached(CDPSession session, PageFrameAttachedResponse frameAttached)
            => OnFrameAttached(session, frameAttached.FrameId, frameAttached.ParentFrameId);

        private void OnFrameAttached(CDPSession session, string frameId, string parentFrameId)
        {
            var frame = GetFrame(frameId);
            if (frame != null)
            {
                if (session != null && frame.IsOopFrame)
                {
                    frame.UpdateClient(session);
                }

                return;
            }

            frame = new Frame(this, frameId, parentFrameId, session);
            FrameTree.AddFrame(frame);
            FrameAttached?.Invoke(this, new FrameEventArgs(frame));
        }

        private async Task HandleFrameTreeAsync(CDPSession session, PageGetFrameTree frameTree)
        {
            if (!string.IsNullOrEmpty(frameTree.Frame.ParentId))
            {
                OnFrameAttached(session, frameTree.Frame.Id, frameTree.Frame.ParentId);
            }

            if (!_frameNavigatedReceived.Contains(frameTree.Frame.Id))
            {
                await OnFrameNavigatedAsync(frameTree.Frame).ConfigureAwait(false);
            }
            else
            {
                _frameNavigatedReceived.Remove(frameTree.Frame.Id);
            }

            if (frameTree.ChildFrames != null)
            {
                foreach (var child in frameTree.ChildFrames)
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
                        WorldName = name,
                    }))).ConfigureAwait(false);
            }
            catch (PuppeteerException ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
