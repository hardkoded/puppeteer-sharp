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

        public FrameManager()
        {
        }
    }
}
