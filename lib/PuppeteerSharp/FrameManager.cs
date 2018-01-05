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
        private Dictionary<string, Frame> _frames;
        private Page _page;

        public FrameManager(Session client, FrameTree frameTree, Page page)
        {
            _client = client;
            _page = page;

            HandleFrameTree(frameTree);
        }

        private void HandleFrameTree(FrameTree frameTree)
        {
            throw new NotImplementedException();
        }

        internal Frame MainFrame()
        {
            throw new NotImplementedException();
        }
    }
}
