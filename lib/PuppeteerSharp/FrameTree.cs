using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class FrameTree
    {
        public FrameTree()
        {
            Childs = new List<Frame>();
        }

        public Frame Frame { get; set; }
        public List<Frame> Childs { get; set; }
    }
}