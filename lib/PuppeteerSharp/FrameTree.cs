using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class FrameTree
    {
        public FrameTree()
        {
            Childs = new List<FrameTree>();
        }

        public Frame Frame { get; set; }
        public List<FrameTree> Childs { get; set; }
    }
}