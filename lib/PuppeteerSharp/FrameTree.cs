using System.Collections.Generic;

namespace PuppeteerSharp
{
    public struct FrameTree
    {
        public FrameTree()
        {
            Childs = new List<Frame>();
        }

        public Frame Frame { get; set; }
        public List<Frame> Childs { get; set; }
    }
}