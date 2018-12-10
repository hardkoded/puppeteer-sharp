using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal class FrameManager
    {
        private readonly CDPSession _client;
        private Dictionary<int, ExecutionContext> _contextIdToContext;
        private bool _ensureNewDocumentNavigation;
        private readonly ILogger _logger;
        private readonly NetworkManager _networkManager;
        private readonly ConcurrentDictionary<string, Frame> _frames;
        private readonly MultiMap<string, TaskCompletionSource<Frame>> _pendingFrameRequests;
        private const int WaitForRequestDelay = 1000;

        private FrameManager(CDPSession client, Page page, NetworkManager networkManager)
        {
            _client = client;
            Page = page;
            _frames = new ConcurrentDictionary<string, Frame>();
            _contextIdToContext = new Dictionary<int, ExecutionContext>();
            _logger = _client.Connection.LoggerFactory.CreateLogger<FrameManager>();
            _networkManager = networkManager;
            _pendingFrameRequests = new MultiMap<string, TaskCompletionSource<Frame>>();

            _client.MessageReceived += Client_MessageReceived;
        }

        #region Properties

        internal event EventHandler<FrameEventArgs> FrameAttached;
        internal event EventHandler<FrameEventArgs> FrameDetached;
        internal event EventHandler<FrameEventArgs> FrameNavigated;
        internal event EventHandler<FrameEventArgs> FrameNavigatedWithinDocument;
        internal event EventHandler<FrameEventArgs> LifecycleEvent;

        internal Frame MainFrame { get; set; }
        internal Page Page { get; }
        internal int DefaultNavigationTimeout { get; set; } = 30000;

        #endregion

        #region Public Methods
        internal static async Task<FrameManager> CreateFrameManagerAsync(CDPSession client, Page page, NetworkManager networkManager, FrameTree frameTree)
        {
            var frameManager = new FrameManager(client, page, networkManager);
            await frameManager.HandleFrameTreeAsync(frameTree).ConfigureAwait(false);
            return frameManager;
        }

        internal ExecutionContext ExecutionContextById(int contextId)
        {
            _contextIdToContext.TryGetValue(contextId, out var context);

            if (context == null)
            {
                _logger.LogError("INTERNAL ERROR: missing context with id = {ContextId}", contextId);
            }
            return context;
        }

        public async Task<Response> NavigateFrameAsync(Frame frame, string url, NavigationOptions options)
        {
            var referrer = string.IsNullOrEmpty(options.Referer)
               ? _networkManager.ExtraHTTPHeaders?.GetValueOrDefault(MessageKeys.Referer)
               : options.Referer;
            var requests = new Dictionary<string, Request>();
            var timeout = options?.Timeout ?? DefaultNavigationTimeout;
            using (var watcher = new NavigatorWatcher(_client, this, frame, _networkManager, timeout, options))
            {
                var navigateTask = NavigateAsync(_client, url, referrer, frame.Id);
                await Task.WhenAny(
                    watcher.TimeoutOrTerminationTask,
                    navigateTask).ConfigureAwait(false);

                AggregateException exception = null;
                if (navigateTask.IsFaulted)
                {
                    exception = navigateTask.Exception;
                }
                else
                {
                    await Task.WhenAny(
                        watcher.TimeoutOrTerminationTask,
                        _ensureNewDocumentNavigation ? watcher.NewDocumentNavigationTask : watcher.SameDocumentNavigationTask
                    ).ConfigureAwait(false);

                    if (watcher.TimeoutOrTerminationTask.IsCompleted && watcher.TimeoutOrTerminationTask.Result.IsFaulted)
                    {
                        exception = watcher.TimeoutOrTerminationTask.Result.Exception;
                    }
                }

                if (exception != null)
                {
                    throw new NavigationException(exception.InnerException.Message, exception.InnerException);
                }

                return watcher.NavigationResponse;
            }
        }

        private async Task NavigateAsync(CDPSession client, string url, string referrer, string frameId)
        {
            var response = await client.SendAsync<PageNavigateResponse>("Page.navigate", new
            {
                url,
                referrer = referrer ?? string.Empty,
                frameId
            }).ConfigureAwait(false);

            _ensureNewDocumentNavigation = !string.IsNullOrEmpty(response.LoaderId);

            if (!string.IsNullOrEmpty(response.ErrorText))
            {
                throw new NavigationException(response.ErrorText, url);
            }
        }

        public async Task<Response> WaitForFrameNavigationAsync(Frame frame, NavigationOptions options = null)
        {
            var timeout = options?.Timeout ?? DefaultNavigationTimeout;
            using (var watcher = new NavigatorWatcher(_client, this, frame, _networkManager, timeout, options))
            {
                var raceTask = await Task.WhenAny(
                    watcher.NewDocumentNavigationTask,
                    watcher.SameDocumentNavigationTask,
                    watcher.TimeoutOrTerminationTask
                ).ConfigureAwait(false);

                var exception = raceTask.Exception;
                if (exception == null &&
                    watcher.TimeoutOrTerminationTask.IsCompleted &&
                    watcher.TimeoutOrTerminationTask.Result.IsFaulted)
                {
                    exception = watcher.TimeoutOrTerminationTask.Result.Exception;
                }
                if (exception != null)
                {
                    throw new NavigationException(exception.Message, exception);
                }

                return watcher.NavigationResponse;
            }
        }

        #endregion

        #region Private Methods

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "Page.frameAttached":
                        OnFrameAttached(
                            e.MessageData.SelectToken(MessageKeys.FrameId).ToObject<string>(),
                            e.MessageData.SelectToken("parentFrameId").ToObject<string>());
                        break;

                    case "Page.frameNavigated":
                        await OnFrameNavigatedAsync(e.MessageData.SelectToken(MessageKeys.Frame).ToObject<FramePayload>(true)).ConfigureAwait(false);
                        break;

                    case "Page.navigatedWithinDocument":
                        OnFrameNavigatedWithinDocument(e.MessageData.ToObject<NavigatedWithinDocumentResponse>(true));
                        break;

                    case "Page.frameDetached":
                        OnFrameDetached(e.MessageData.ToObject<BasicFrameResponse>(true));
                        break;

                    case "Page.frameStoppedLoading":
                        OnFrameStoppedLoading(e.MessageData.ToObject<BasicFrameResponse>(true));
                        break;

                    case "Runtime.executionContextCreated":
                        await OnExecutionContextCreatedAsync(e.MessageData.SelectToken(MessageKeys.Context).ToObject<ContextPayload>(true));
                        break;

                    case "Runtime.executionContextDestroyed":
                        OnExecutionContextDestroyed(e.MessageData.SelectToken(MessageKeys.ExecutionContextId).ToObject<int>());
                        break;
                    case "Runtime.executionContextsCleared":
                        OnExecutionContextsCleared();
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
                _client.Close(message);
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

        private void OnExecutionContextsCleared()
        {
            while (_contextIdToContext.Count > 0)
            {
                var contextItem = _contextIdToContext.ElementAt(0);
                RemoveContext(contextItem.Value);
                _contextIdToContext.Remove(contextItem.Key);
            }
        }

        private void OnExecutionContextDestroyed(int executionContextId)
        {
            _contextIdToContext.TryGetValue(executionContextId, out var context);

            if (context != null)
            {
                _contextIdToContext.Remove(executionContextId);
                RemoveContext(context);
            }
        }

        private async Task OnExecutionContextCreatedAsync(ContextPayload contextPayload)
        {
            var frameId = contextPayload.AuxData.IsDefault ? contextPayload.AuxData.FrameId : null;
            var frame = !string.IsNullOrEmpty(frameId) ? await GetFrameAsync(frameId) : null;

            var context = new ExecutionContext(
                _client,
                contextPayload,
                frame);

            _contextIdToContext[contextPayload.Id] = context;

            if (frame != null)
            {
                frame.SetDefaultContext(context);
            }
        }

        private void OnFrameDetached(BasicFrameResponse e)
        {
            if (_frames.TryGetValue(e.FrameId, out var frame))
            {
                RemoveFramesRecursively(frame);
            }
        }

        private async Task OnFrameNavigatedAsync(FramePayload framePayload)
        {
            var isMainFrame = string.IsNullOrEmpty(framePayload.ParentId);
            var frame = isMainFrame ? MainFrame : await GetFrameAsync(framePayload.Id);

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
                    frame = new Frame(this, _client, null, framePayload.Id);
                }
                AddFrame(framePayload.Id, frame);
                MainFrame = frame;
            }

            // Update frame payload.
            frame.Navigated(framePayload);

            FrameNavigated?.Invoke(this, new FrameEventArgs(frame));
        }

        private void AddFrame(string frameId, Frame frame)
        {
            _frames[frameId] = frame;
            foreach (var tcs in _pendingFrameRequests.Get(frameId))
            {
                tcs.TrySetResult(frame);
            }
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

        private void RemoveContext(ExecutionContext context)
        {
            if (context.Frame != null)
            {
                context.Frame.SetDefaultContext(null);
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

        private void OnFrameAttached(string frameId, string parentFrameId)
        {
            if (!_frames.ContainsKey(frameId) && _frames.ContainsKey(parentFrameId))
            {
                var parentFrame = _frames[parentFrameId];
                var frame = new Frame(this, _client, parentFrame, frameId);
                _frames[frame.Id] = frame;
                FrameAttached?.Invoke(this, new FrameEventArgs(frame));
            }
        }

        private async Task HandleFrameTreeAsync(FrameTree frameTree)
        {
            if (!string.IsNullOrEmpty(frameTree.Frame.ParentId))
            {
                OnFrameAttached(frameTree.Frame.Id, frameTree.Frame.ParentId);
            }

            await OnFrameNavigatedAsync(frameTree.Frame);

            if (frameTree.Childs != null)
            {
                foreach (var child in frameTree.Childs)
                {
                    await HandleFrameTreeAsync(child);
                }
            }
        }

        internal async Task<Frame> GetFrameAsync(string frameId)
        {
            var tcs = new TaskCompletionSource<Frame>();
            _pendingFrameRequests.Add(frameId, tcs);

            if (_frames.TryGetValue(frameId, out var frame))
            {
                _pendingFrameRequests.Delete(frameId, tcs);
                return frame;
            }

            var delayTask = Task.Delay(WaitForRequestDelay);
            var task = Task.WhenAny(
                delayTask,
                tcs.Task
            );

            if (task == delayTask)
            {
                throw new PuppeteerException($"Frame '{frameId}' not found");
            }

            return await tcs.Task;
        }

        #endregion
    }
}