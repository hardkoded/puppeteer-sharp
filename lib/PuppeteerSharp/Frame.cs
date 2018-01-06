using System;
using System.Collections.Generic;
using PuppeteerSharp.Input;

namespace PuppeteerSharp
{
    public class Frame
    {
        private Session _client;
        private Page _page;
        private Frame _parentFrame;
        private string _id;
        private string _defaultContextId = "<not-initialized>";
        private object _context = null;

        public Frame(Session client, Page page, Frame parentFrame, string frameId)
        {
            _client = client;
            _page = page;
            _id = frameId;
            _parentFrame = parentFrame;

            if (parentFrame != null)
            {
                _parentFrame.ChildFrames.Add(this);
            }
        }

        public List<Frame> ChildFrames { get; set; } = new List<Frame>();
        public object ExecutionContext => _context;
        public string Url { get; set; }


    }
}
