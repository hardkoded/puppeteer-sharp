using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using PuppeteerSharp.Input;

namespace PuppeteerSharp
{
    public class FrameManager
    {
        private Session _client;
        private Mouse _mouse;
        private Touchscreen _touchscreen;
        private Page _page;
        private Dictionary<string, ExecutionContext> _contextIdToContext;

        public FrameManager(Session client, FrameTree frameTree, Page page)
        {
            _client = client;
            _page = page;
            Frames = new Dictionary<string, Frame>();
            _contextIdToContext = new Dictionary<string, ExecutionContext>();

            _client.MessageReceived += _client_MessageReceived;
            HandleFrameTree(frameTree);

        }

        #region Properties
        public event EventHandler<FrameEventArgs> FrameAttached;
        public event EventHandler<EventArgs> FrameDetached;
        public event EventHandler<FrameEventArgs> FrameNavigated;
        public event EventHandler<FrameEventArgs> LifecycleEvent;

        public Dictionary<string, Frame> Frames { get; internal set; }
        public Frame MainFrame { get; internal set; }

        #endregion

        #region Private Methods

        void _client_MessageReceived(object sender, PuppeteerSharp.MessageEventArgs e)
        {
            switch (e.MessageID)
            {
                case "Page.frameAttached":
                    OnFrameAttached(e.FrameId, e.ParentFrameId);
                    break;

                case "Page.frameNavigated":
                    OnFrameNavigated(e.Frame);
                    break;

                case "Page.frameDetached":
                    OnFrameDetached(e.FrameId);
                    break;

                case "Runtime.executionContextCreated":
                    OnExecutionContextCreated(e.Context);
                    break;

                case "Runtime.executionContextDestroyed":
                    OnExecutionContextDestroyed(e.ExecutionContextId);
                    break;
                case "Runtime.executionContextsCleared":
                    OnExecutionContextsCleared();
                    break;
                case "Page.lifecycleEvent":
                    OnLifeCycleEvent(e);
                    break;
            }

        }

        private void OnLifeCycleEvent(MessageEventArgs e)
        {
            if (!Frames.ContainsKey((e.FrameId)))
            {
                var frame = Frames[e.FrameId];

                frame.OnLifecycleEvent(e.LoaderId, e.Name);
                LifecycleEvent?.Invoke(this, new FrameEventArgs(frame));
            }
        }

        private void OnExecutionContextsCleared()
        {
            throw new NotImplementedException();
        }

        private void OnExecutionContextDestroyed(string executionContextId)
        {
            throw new NotImplementedException();
        }

        private void OnExecutionContextCreated(ContextPayload context)
        {
            /*
            var context = new ExecutionContext(_client, contextPayload, this.createJSHandle.bind(this, contextPayload.id));
            this._contextIdToContext.set(contextPayload.id, context);

            const frame = context._frameId ? this._frames.get(context._frameId) : null;
            if (frame && context._isDefault)
                frame._setDefaultContext(context);
                */
        }

        private void OnFrameDetached(string frameId)
        {
            if (Frames.ContainsKey(frameId))
            {
                RemoveFramesRecursively(Frames[frameId]);
            }
        }

        private void OnFrameNavigated(Frame frame)
        {
            throw new NotImplementedException();
        }

        private void OnFrameNavigated(FramePayload framePayload)
        {
            var isMainFrame = string.IsNullOrEmpty(framePayload.ParentId);
            var frame = isMainFrame ? MainFrame : Frames[framePayload.Id];

            Contract.Assert(isMainFrame || frame != null, "We either navigate top level or have old version of the navigated frame");

            // Detach all child frames first.
            if (frame != null)
            {
                foreach (var child in frame.ChildFrames)
                {
                    RemoveFramesRecursively(child);
                }
            }

            // Update or create main frame.
            if (isMainFrame)
            {
                if (frame != null)
                {
                    // Update frame id to retain frame identity on cross-process navigation.
                    Frames.Remove(frame.Id);
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

        private void RemoveFramesRecursively(Frame child)
        {
            throw new NotImplementedException();
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
