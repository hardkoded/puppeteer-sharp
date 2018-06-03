using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using PuppeteerSharp.Input;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace PuppeteerSharp
{
    internal class FrameManager
    {
        private readonly CDPSession _client;
        private readonly Page _page;
        private Dictionary<int, ExecutionContext> _contextIdToContext;
        private readonly ILogger _logger;

        internal FrameManager(CDPSession client, FrameTree frameTree, Page page)
        {
            _client = client;
            _page = page;
            Frames = new Dictionary<string, Frame>();
            _contextIdToContext = new Dictionary<int, ExecutionContext>();
            _logger = _client.Connection.LoggerFactory.CreateLogger<FrameManager>();

            _client.MessageReceived += _client_MessageReceived;
            HandleFrameTree(frameTree);
        }

        #region Properties
        internal event EventHandler<FrameEventArgs> FrameAttached;
        internal event EventHandler<FrameEventArgs> FrameDetached;
        internal event EventHandler<FrameEventArgs> FrameNavigated;
        internal event EventHandler<FrameEventArgs> LifecycleEvent;

        internal Dictionary<string, Frame> Frames { get; set; }
        internal Frame MainFrame { get; set; }

        #endregion

        #region Private Methods

        void _client_MessageReceived(object sender, PuppeteerSharp.MessageEventArgs e)
        {
            switch (e.MessageID)
            {
                case "Page.frameAttached":
                    OnFrameAttached(e.MessageData.frameId.ToString(), e.MessageData.parentFrameId.ToString());
                    break;

                case "Page.frameNavigated":
                    OnFrameNavigated(((JObject)e.MessageData.frame).ToObject<FramePayload>());
                    break;

                case "Page.frameDetached":
                    OnFrameDetached(e.MessageData.frameId.ToString());
                    break;

                case "Runtime.executionContextCreated":
                    OnExecutionContextCreated(new ContextPayload(e.MessageData.context));
                    break;

                case "Runtime.executionContextDestroyed":
                    OnExecutionContextDestroyed((int)e.MessageData.executionContextId);
                    break;
                case "Runtime.executionContextsCleared":
                    OnExecutionContextsCleared();
                    break;
                case "Page.lifecycleEvent":
                    OnLifeCycleEvent(e);
                    break;
                default:
                    break;
            }
        }

        private void OnLifeCycleEvent(MessageEventArgs e)
        {
            if (Frames.ContainsKey(e.MessageData.frameId.ToString()))
            {
                Frame frame = Frames[e.MessageData.frameId.ToString()];

                frame.OnLifecycleEvent(e.MessageData.loaderId.ToString(), e.MessageData.name.ToString());
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

        public JSHandle CreateJsHandle(int contextId, dynamic remoteObject)
        {
            _contextIdToContext.TryGetValue(contextId, out var storedContext);

            if (storedContext == null)
            {
                _logger.LogError("INTERNAL ERROR: missing context with id = {ContextId}", contextId);
            }

            if (remoteObject.subtype == "node")
            {
                return new ElementHandle(storedContext, _client, remoteObject, _page);
            }

            return new JSHandle(storedContext, _client, remoteObject);
        }

        private void OnExecutionContextCreated(ContextPayload contextPayload)
        {
            var context = new ExecutionContext(_client, contextPayload,
                remoteObject => CreateJsHandle(contextPayload.Id, remoteObject));

            _contextIdToContext[contextPayload.Id] = context;

            var frame = !string.IsNullOrEmpty(context.FrameId) ? Frames[context.FrameId] : null;
            if (frame != null && context.IsDefault)
            {
                frame.SetDefaultContext(context);
            }
        }

        private void OnFrameDetached(string frameId)
        {
            if (Frames.ContainsKey(frameId))
            {
                RemoveFramesRecursively(Frames[frameId]);
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
                    frame = new Frame(this._client, this._page, null, framePayload.Id);
                }

                Frames[framePayload.Id] = frame;
                MainFrame = frame;
            }

            // Update frame payload.
            frame.Navigated(framePayload);

            FrameNavigated?.Invoke(this, new FrameEventArgs(frame));
        }

        private void RemoveContext(ExecutionContext context)
        {
            var frame = !string.IsNullOrEmpty(context.FrameId) ? Frames[context.FrameId] : null;

            if (frame != null && context.IsDefault)
            {
                frame.SetDefaultContext(null);
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
                var frame = new Frame(_client, _page, parentFrame, frameId);
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
