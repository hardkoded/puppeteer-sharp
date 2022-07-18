using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace CefSharp.DevTools.Dom.Messaging
{
    internal class PageGetFrameTreeResponse
    {
        public PageGetFrameTreeItem FrameTree { get; set; }
    }
}
