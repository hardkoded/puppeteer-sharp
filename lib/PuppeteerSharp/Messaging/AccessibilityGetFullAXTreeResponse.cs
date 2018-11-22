using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Messaging
{
    internal class AccessibilityGetFullAXTreeResponse
    {
        public IEnumerable<AXTreeNode> Nodes { get; set; }

        public class AXTreeNode
        {
            public string NodeId { get; set; }
            public IEnumerable<string> ChildIds { get; set; }
            public AXTreePropertyValue Name { get; set; }
            public AXTreePropertyValue Value { get; set; }
            public AXTreePropertyValue Description { get; set; }
            public AXTreePropertyValue Role { get; set; }
            public IEnumerable<AXTreeProperty> Properties { get; set; }
        }

        public class AXTreeProperty
        {
            public string Name { get; set; }
            public AXTreePropertyValue Value { get; set; }
        }

        public class AXTreePropertyValue
        {
            public string Type { get; set; }
            public JToken Value { get; set; }
        }
    }
}
