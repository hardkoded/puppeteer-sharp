using System;
using System.Collections.Generic;

namespace PuppeteerSharp.Messaging
{
    internal class AccessibilityGetFullAXTreeResponse
    {
        public string NodeId { get; set; }
        public IEnumerable<string> ChildIds { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public IEnumerable<AXTreeProperty> Properties { get; set; }

        public class AXTreeProperty
        {
            public string Name { get; internal set; }
            public AXTreePropertyValue Value { get; set; }

            public class AXTreePropertyValue
            {
                public string Value { get; set; }
            }
        }
    }
}
