using System;
using System.Collections.Generic;
using PuppeteerSharp.Input;

namespace PuppeteerSharp
{
    public class Frame
    {
        private Session _client;
        private Mouse _mouse;
        private Touchscreen _touchscreen;
        private Frame _parentFrame;
        private string _url = string.Empty;
        private string _id;
        private string _defaultContextId = "<not-initialized>";

        public Frame(Session client, Mouse mouse, Touchscreen touchscreen, Frame parentFrame, string frameId)
        {
            _client = client;
            _mouse = mouse;
            _touchscreen = touchscreen;
            _id = frameId;
            _parentFrame = parentFrame;

            if(parentFrame != null)
            {
                _parentFrame.ChildFrames.Add(this);
            }
        }

		public List<Frame> ChildFrames = new List<Frame>();


	}
}
