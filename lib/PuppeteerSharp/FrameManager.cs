using System;
using System.Collections.Generic;
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
        public event EventHandler<EventArgs> FrameAttached;
        public event EventHandler<EventArgs> FrameDetached;
        public event EventHandler<EventArgs> FrameNavigated;
        public event EventHandler<FrameEventArgs> LifecycleEvent;

        public Dictionary<string, Frame> Frames { get; internal set; }

        #endregion

        public Frame MainFrame()
        {
            throw new NotImplementedException();
        }

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
                //this.emit(FrameManager.Events.LifecycleEvent, frame);
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

        private void OnExecutionContextCreated(ContextData context)
        {
            throw new NotImplementedException();
        }

        private void OnFrameDetached(string frameId)
        {
            throw new NotImplementedException();
        }

        private void OnFrameNavigated(FrameData frame)
        {
            throw new NotImplementedException();
        }

        private void OnFrameAttached(string frameId, string parentFrameId)
        {
            throw new NotImplementedException();
        }

        private void HandleFrameTree(FrameTree frameTree)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
