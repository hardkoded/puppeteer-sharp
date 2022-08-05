using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.PageAccessibility;
using static PuppeteerSharp.Messaging.AccessibilityGetFullAXTreeResponse;

namespace PuppeteerSharp.Messaging
{
    internal class AccessibilityQueryAXTreeResponse
    {
        public IEnumerable<AXTreeNode> Nodes { get; set; }
    }
}
