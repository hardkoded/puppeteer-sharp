using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Messaging
{
    internal class AccessibilityGetFullAXTreeResponse
    {
        [JsonProperty("nodes")]
        public IEnumerable<AXTreeNode> Nodes { get; set; }

        public class AXTreeNode
        {
            [JsonProperty("nodeId")]
            public string NodeId { get; set; }
            [JsonProperty("childIds")]
            public IEnumerable<string> ChildIds { get; set; }
            [JsonProperty("name")]
            public AXTreePropertyValue Name { get; set; }
            [JsonProperty("value")]
            public AXTreePropertyValue Value { get; set; }
            [JsonProperty("description")]
            public AXTreePropertyValue Description { get; set; }
            [JsonProperty("role")]
            public AXTreePropertyValue Role { get; set; }
            [JsonProperty("properties")]
            public IEnumerable<AXTreeProperty> Properties { get; set; }
        }

        public class AXTreeProperty
        {
            [JsonProperty("name")]
            public string Name { get; internal set; }
            [JsonProperty("value")]
            public AXTreePropertyValue Value { get; set; }
        }

        public class AXTreePropertyValue
        {

            [JsonProperty("type")]
            public string Type { get; set; }
            [JsonProperty("value")]
            public JToken Value { get; set; }
        }
    }
}
