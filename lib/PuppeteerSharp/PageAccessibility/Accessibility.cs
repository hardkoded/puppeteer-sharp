using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp.PageAccessibility
{
    /// <summary>
    /// The Accessibility class provides methods for inspecting Chromium's accessibility tree. 
    /// The accessibility tree is used by assistive technology such as screen readers.
    /// 
    /// Accessibility is a very platform-specific thing. On different platforms, there are different screen readers that might have wildly different output.
    /// Blink - Chrome's rendering engine - has a concept of "accessibility tree", which is than translated into different platform-specific APIs. 
    /// Accessibility namespace gives users access to the Blink Accessibility Tree.
    /// Most of the accessibility tree gets filtered out when converting from Blink AX Tree to Platform-specific AX-Tree or by screen readers themselves. 
    /// By default, Puppeteer tries to approximate this filtering, exposing only the "interesting" nodes of the tree.
    /// </summary>
    public class Accessibility
    {
        private readonly CDPSession _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="PuppeteerSharp.PageAccessibility.Accessibility"/> class.
        /// </summary>
        /// <param name="client">Client.</param>
        public Accessibility(CDPSession client) => _client = client;

        /// <summary>
        /// Snapshots the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="options">Options.</param>
        public async Task<SerializedAXNode> SnapshotAsync(AccessibilitySnapshotOptions options = null)
        {
            var nodes = (await _client.SendAsync<AccessibilityGetFullAXTreeResponse>("Accessibility.getFullAXTree")).Nodes;
            var root = AXNode.CreateTree(nodes);
            if (options?.InterestingOnly == false)
            {
                return SerializeTree(root)[0];
            }

            var interestingNodes = new List<AXNode>();
            CollectInterestingNodes(interestingNodes, root, false);
            return SerializeTree(root, interestingNodes)[0];
        }

        private void CollectInterestingNodes(List<AXNode> collection, AXNode node, bool insideControl)
        {
            if (node.IsInteresting(insideControl))
            {
                collection.Add(node);
            }
            if (node.IsLeafNode())
            {
                return;
            }
            insideControl = insideControl || node.IsControl();
            foreach (var child in node.Children)
            {
                CollectInterestingNodes(collection, child, insideControl);
            }
        }

        private SerializedAXNode[] SerializeTree(AXNode node, List<AXNode> whitelistedNodes = null)
        {
            var children = new List<SerializedAXNode>();
            foreach (var child in node.Children)
            {
                children.AddRange(SerializeTree(child, whitelistedNodes));
            }
            if (whitelistedNodes?.Contains(node) == false)
            {
                return children.ToArray();
            }

            var serializedNode = node.Serialize();
            if (children.Count > 0)
            {
                serializedNode.Children = children.ToArray();
            }
            return new[] { serializedNode };
        }
    }
}