using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Messaging
{
    internal class PageGetFrameTreeResponse
    {
        public PageGetFrameTreeItem FrameTree { get; set; }
    }

    internal class PageGetFrameTreeItem
    {
        public PageGetFrameTreeItemInfo Frame { get; set; }
        public IEnumerable<PageGetFrameTreeItem> ChildFrames { get; set; }
    }

    internal class PageGetFrameTreeItemInfo
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
