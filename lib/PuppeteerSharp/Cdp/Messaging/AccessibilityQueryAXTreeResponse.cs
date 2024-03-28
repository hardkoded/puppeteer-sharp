using System.Collections.Generic;
using static PuppeteerSharp.Cdp.Messaging.AccessibilityGetFullAXTreeResponse;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class AccessibilityQueryAXTreeResponse
    {
        public IEnumerable<AXTreeNode> Nodes { get; set; }
    }
}
