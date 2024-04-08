using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp
{
    internal class FrameManager : IDisposable, IAsyncDisposable, IFrameProvider
    {
        private const int TimeForWaitingForSwap = 200;
        private const string UtilityWorldName = "__puppeteer_utility_world__";

        private readonly ConcurrentDictionary<string, ExecutionContext> _contextIdToContext = new();
        private readonly ILogger _logger;
        private readonly List<string> _isolatedWorlds = [];
        private readonly List<string> _frameNavigatedReceived = [];
        private readonly TaskQueue _eventsQueue = new();
        private readonly ConcurrentDictionary<CDPSession, DeviceRequestPromptManager> _deviceRequestPromptManagerMap = new();
        private TaskCompletionSource<bool> _frameTreeHandled = new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal FrameManager(CDPSession client, Page page, bool ignoreHTTPSErrors, TimeoutSettings timeoutSettings)
        {
            Client = client;
            Page = page;
            _logger = Client.Connection.LoggerFactory.CreateLogger<FrameManager>();
            NetworkManager = new NetworkManager(ignoreHTTPSErrors, this, client.Connection.LoggerFactory);
            TimeoutSettings = timeoutSettings;

            Client.MessageReceived += Client_MessageReceived;
            Client.Disconnected += (sender, e) => _ = OnClientDisconnectAsync();
        }

        internal event EventHandler<FrameEventArgs> FrameAttached;

        internal event EventHandler<FrameEventArgs> FrameDetached;

        internal event EventHandler<FrameEventArgs> FrameSwapped;

        internal event EventHandler<FrameNavigatedEventArgs> FrameNavigated;

        internal event EventHandler<FrameEventArgs> FrameNavigatedWithinDocument;

        internal event EventHandler<FrameEventArgs> LifecycleEvent;

        internal CDPSession Client { get; private set; }

        internal NetworkManager NetworkManager { get; }

        internal Page Page { get; }

        internal TimeoutSettings TimeoutSettings { get; }

        internal FrameTree FrameTree { get; } = new();

        internal Frame MainFrame => FrameTree.MainFrame;

        public void Dispose() => _eventsQueue?.Dispose();

        public async ValueTask DisposeAsync()
        {
            if (_eventsQueue != null)
            {
                await _eventsQueue.DisposeAsync().ConfigureAwait(false);
            }
        }

        public Task<CdpFrame> GetFrameAsync(string frameId) => FrameTree.TryGetFrameAsync(frameId);

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

        internal ExecutionContext GetExecutionContextById(int contextId, CDPSession session)
        {
            _contextIdToContext.TryGetValue($"{session.Id}:{contextId}", out var context);
            return context;
        }

        internal DeviceRequestPromptManager GetDeviceRequestPromptManager(CDPSession client)
            => _deviceRequestPromptManagerMap.GetOrAdd(client, _ => new DeviceRequestPromptManager(client, TimeoutSettings));

        internal Frame[] GetFrames() => FrameTree.Frames;

        internal async Task InitializeAsync(CDPSession client)
        {
            try
            {
                _frameTreeHandled.TrySetResult(true);
                _frameTreeHandled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                var networkInitTask = NetworkManager.AddClientAsync(client);
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

                _frameTreeHandled.TrySetResult(true);
                await HandleFrameTreeAsync(client, getFrameTreeTask.Result.FrameTree).ConfigureAwait(false);

                await Task.WhenAll(
                    client.SendAsync("Page.setLifecycleEventsEnabled", new PageSetLifecycleEventsEnabledRequest { Enabled = true }),
                    client.SendAsync("Runtime.enable"),
                    networkInitTask).ConfigureAwait(false);

                await CreateIsolatedWorldAsync(client, UtilityWorldName).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _frameTreeHandled.TrySetResult(true);

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

        /// <summary>
        /// When the main frame is replaced by another main frame
        /// we maintain the main frame object identity while updating
        /// its frame tree and ID.
        /// </summary>
        /// <param name="client">New session.</param>
        internal async Task SwapFrameTreeAsync(CDPSession client)
        {
            OnExecutionContextsCleared(Client);

            Client = client;

            var frame = FrameTree.MainFrame;
            if (frame != null)
            {
                _frameNavigatedReceived.Add(Client.Target.TargetId);
                FrameTree.RemoveFrame(frame);
                frame.Id = Client.Target.TargetId;
                frame.MainWorld.ClearContext();
                frame.PuppeteerWorld.ClearContext();
                FrameTree.AddFrame(frame);
                frame.UpdateClient(client, true);
            }

            Client.MessageReceived += Client_MessageReceived;
            Client.Disconnected += (sender, e) => _ = OnClientDisconnectAsync();

            await InitializeAsync(client).ConfigureAwait(false);
            await NetworkManager.AddClientAsync(client).ConfigureAwait(false);

            frame?.OnFrameSwappedByActivation();
        }

        internal Task RegisterSpeculativeSessionAsync(CDPSession client)
            => NetworkManager.AddClientAsync(client);

        private CdpFrame GetFrame(string frameId) => FrameTree.GetById(frameId);

        private void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            _ = _eventsQueue.Enqueue(async () =>
            {
                try
                {
                    await _frameTreeHandled.Task.WithTimeout().ConfigureAwait(false);
                    switch (e.MessageID)
                    {
                        case "Page.frameAttached":
                            OnFrameAttached(sender as CDPSession, e.MessageData.ToObject<PageFrameAttachedResponse>());
                            break;

                        case "Page.frameNavigated":
                            var response = e.MessageData.ToObject<PageFrameNavigatedResponse>(true);
                            await OnFrameNavigatedAsync(response.Frame, response.Type).ConfigureAwait(false);
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
#pragma warning disable CA2000
            if (_contextIdToContext.TryRemove(key, out var context))
#pragma warning restore CA2000
            {
                context.World?.ClearContext();
            }
        }

        private async Task OnExecutionContextCreatedAsync(ContextPayload contextPayload, ICDPSession session)
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

            // If there is no world, the context is not meant to be handled by us.
            if (world == null)
            {
                return;
            }

            var context = new ExecutionContext(frame.Client ?? Client, contextPayload, world);
            world.SetContext(context);

            var key = $"{session.Id}:{contextPayload.Id}";
            _contextIdToContext[key] = context;
        }

        private void OnFrameDetached(PageFrameDetachedResponse e)
        {
            var frame = GetFrame(e.FrameId);
            if (frame == null)
            {
                return;
            }

            if (e.Reason == FrameDetachedReason.Remove)
            {
                RemoveFramesRecursively(frame);
            }
            else if (e.Reason == FrameDetachedReason.Swap)
            {
                FrameSwapped?.Invoke(frame, new FrameEventArgs(frame));
                frame.OnSwapped();
            }
        }

        private async Task OnFrameNavigatedAsync(FramePayload framePayload, NavigationType type)
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
                while (frame.ChildFrames.Count > 0)
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
                    frame = new CdpFrame(this, framePayload.Id, null, Client);
                }

                FrameTree.AddFrame((CdpFrame)frame);
            }

            // Update frame payload.
            frame.Navigated(framePayload);
            frame.OnFrameNavigated(new FrameNavigatedEventArgs(frame, type));
            FrameNavigated?.Invoke(this, new FrameNavigatedEventArgs(frame, type));
        }

        private void OnFrameNavigatedWithinDocument(NavigatedWithinDocumentResponse e)
        {
            var frame = GetFrame(e.FrameId);
            if (frame != null)
            {
                frame.NavigatedWithinDocument(e.Url);

                var eventArgs = new FrameEventArgs(frame);
                FrameNavigatedWithinDocument?.Invoke(this, eventArgs);
                frame.OnFrameNavigated(new FrameNavigatedEventArgs(frame, NavigationType.Navigation));
                FrameNavigated?.Invoke(this, new FrameNavigatedEventArgs(frame, NavigationType.Navigation));
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

            frame = new CdpFrame(this, frameId, parentFrameId, session);
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
                await OnFrameNavigatedAsync(frameTree.Frame, NavigationType.Navigation).ConfigureAwait(false);
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

        private async Task CreateIsolatedWorldAsync(CDPSession session, string name)
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

        private async Task OnClientDisconnectAsync()
        {
            try
            {
                var mainFrame = FrameTree.MainFrame;
                if (mainFrame == null)
                {
                    return;
                }

                foreach (var child in mainFrame.ChildFrames)
                {
                    RemoveFramesRecursively(child as Frame);
                }

                var swappedTcs = new TaskCompletionSource<bool>();

                mainFrame.FrameSwappedByActivation += (_, _) => swappedTcs.TrySetResult(true);

                try
                {
                    await swappedTcs.Task.WithTimeout(TimeForWaitingForSwap).ConfigureAwait(false);
                }
                catch
                {
                    RemoveFramesRecursively(mainFrame);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while disconnecting");
            }
        }
    }
}
