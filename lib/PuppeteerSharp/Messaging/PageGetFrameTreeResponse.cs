using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Messaging
{
    internal class PageGetFrameTreeResponse
    {
        public PageGetFrameTreeItem FrameTree { get; set; }
    }
}
