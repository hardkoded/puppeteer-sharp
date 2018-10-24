using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

        internal FrameManager(CDPSession client, FrameTree frameTree, Page page, NetworkManager networkManager)
        {
            _client = client;
            Page = page;
            Frames = new Dictionary<string, Frame>();
            _contextIdToContext = new Dictionary<int, ExecutionContext>();
            _logger = _client.Connection.LoggerFactory.CreateLogger<FrameManager>();
            _networkManager = networkManager;

            _client.MessageReceived += _client_MessageReceived;
            HandleFrameTree(frameTree);
        }

        #region Properties

        internal event EventHandler<FrameEventArgs> FrameAttached;
        internal event EventHandler<FrameEventArgs> FrameDetached;
        internal event EventHandler<FrameEventArgs> FrameNavigated;
        internal event EventHandler<FrameEventArgs> FrameNavigatedWithinDocument;
        internal event EventHandler<FrameEventArgs> LifecycleEvent;

        internal Dictionary<string, Frame> Frames { get; set; }
        internal Frame MainFrame { get; set; }
        internal Page Page { get; }
        internal int DefaultNavigationTimeout { get; set; } = 30000;

        #endregion

        #region Public Methods

        internal ExecutionContext ExecutionContextById(int contextId)
        {
            _contextIdToContext.TryGetValue(contextId, out var context);

            if (context == null)
            {
                _logger.LogError("INTERNAL ERROR: missing context with id = {ContextId}", contextId);
            }
            return context;
        }

        public async Task<Response> NavigateAsync(Frame frame, string url, NavigationOptions options)
        {
            var referrer = string.IsNullOrEmpty(options.Referer)
               ? _networkManager.ExtraHTTPHeaders?.GetValueOrDefault(MessageKeys.Referer)
               : options.Referer;
            var requests = new Dictionary<string, Request>();
            var timeout = options?.Timeout ?? DefaultNavigationTimeout;
            var watcher = new NavigatorWatcher(_client, this, frame, _networkManager, timeout, options);

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

        public async Task<Response> WaitForNavigationAsync(Frame frame, NavigationOptions options = null)
        {
            var timeout = options?.Timeout ?? DefaultNavigationTimeout;
            var watcher = new NavigatorWatcher(_client, this, frame, _networkManager, timeout, options);

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

        #endregion

        #region Private Methods

        private void _client_MessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.MessageID)
            {
                case "Page.frameAttached":
                    OnFrameAttached(
                        e.MessageData.SelectToken(MessageKeys.FrameId).ToObject<string>(),
                        e.MessageData.SelectToken("parentFrameId").ToObject<string>());
                    break;

                case "Page.frameNavigated":
                    OnFrameNavigated(e.MessageData.SelectToken(MessageKeys.Frame).ToObject<FramePayload>());
                    break;

                case "Page.navigatedWithinDocument":
                    OnFrameNavigatedWithinDocument(e.MessageData.ToObject<NavigatedWithinDocumentResponse>());
                    break;

                case "Page.frameDetached":
                    OnFrameDetached(e.MessageData.ToObject<BasicFrameResponse>());
                    break;

                case "Page.frameStoppedLoading":
                    OnFrameStoppedLoading(e.MessageData.ToObject<BasicFrameResponse>());
                    break;

                case "Runtime.executionContextCreated":
                    OnExecutionContextCreated(e.MessageData.SelectToken(MessageKeys.Context).ToObject<ContextPayload>());
                    break;

                case "Runtime.executionContextDestroyed":
                    OnExecutionContextDestroyed(e.MessageData.SelectToken(MessageKeys.ExecutionContextId).ToObject<int>());
                    break;
                case "Runtime.executionContextsCleared":
                    OnExecutionContextsCleared();
                    break;
                case "Page.lifecycleEvent":
                    OnLifeCycleEvent(e.MessageData.ToObject<LifecycleEventResponse>());
                    break;
                default:
                    break;
            }
        }

        private void OnFrameStoppedLoading(BasicFrameResponse e)
        {
            if (Frames.TryGetValue(e.FrameId, out var frame))
            {
                frame.OnLoadingStopped();
                LifecycleEvent?.Invoke(this, new FrameEventArgs(frame));
            }
        }

        private void OnLifeCycleEvent(LifecycleEventResponse e)
        {
            if (Frames.TryGetValue(e.FrameId, out var frame))
            {
                frame.OnLifecycleEvent(e.LoaderId, e.Name);
                LifecycleEvent?.Invoke(this, new FrameEventArgs(frame));
            }
        }

        private void OnExecutionContextsCleared()
        {
            foreach (var context in _contextIdToContext.Values)
            {
                RemoveContext(context);
            }
            _contextIdToContext.Clear();
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

        private void OnExecutionContextCreated(ContextPayload contextPayload)
        {
            var frameId = contextPayload.AuxData.IsDefault ? contextPayload.AuxData.FrameId : null;
            var frame = !string.IsNullOrEmpty(frameId) ? Frames[frameId] : null;

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
            if (Frames.TryGetValue(e.FrameId, out var frame))
            {
                RemoveFramesRecursively(frame);
            }
        }

        private void OnFrameNavigated(FramePayload framePayload)
        {
            var isMainFrame = string.IsNullOrEmpty(framePayload.ParentId);
            var frame = isMainFrame ? MainFrame : Frames[framePayload.Id];

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
                        Frames.Remove(frame.Id);
                    }
                    frame.Id = framePayload.Id;
                }
                else
                {
                    // Initial main frame navigation.
                    frame = new Frame(this, _client, null, framePayload.Id);
                }

                Frames[framePayload.Id] = frame;
                MainFrame = frame;
            }

            // Update frame payload.
            frame.Navigated(framePayload);

            FrameNavigated?.Invoke(this, new FrameEventArgs(frame));
        }

        private void OnFrameNavigatedWithinDocument(NavigatedWithinDocumentResponse e)
        {
            if (Frames.TryGetValue(e.FrameId, out var frame))
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
            Frames.Remove(frame.Id);
            FrameDetached?.Invoke(this, new FrameEventArgs(frame));
        }

        private void OnFrameAttached(string frameId, string parentFrameId)
        {
            if (!Frames.ContainsKey(frameId) && Frames.ContainsKey(parentFrameId))
            {
                var parentFrame = Frames[parentFrameId];
                var frame = new Frame(this, _client, parentFrame, frameId);
                Frames[frame.Id] = frame;
                FrameAttached?.Invoke(this, new FrameEventArgs(frame));
            }
        }

        private void HandleFrameTree(FrameTree frameTree)
        {
            if (!string.IsNullOrEmpty(frameTree.Frame.ParentId))
            {
                OnFrameAttached(frameTree.Frame.Id, frameTree.Frame.ParentId);
            }

            OnFrameNavigated(frameTree.Frame);

            if (frameTree.Childs != null)
            {
                foreach (var child in frameTree.Childs)
                {
                    HandleFrameTree(child);
                }
            }
        }

        #endregion
    }
}