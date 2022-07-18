using System.Collections.Generic;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom.Messaging;

namespace CefSharp.DevTools.Dom.PageAccessibility
{
    /// <summary>
    /// The Accessibility class provides methods for inspecting Chromium's accessibility tree.
    /// The accessibility tree is used by assistive technology such as screen readers or switches.
    ///
    /// Accessibility is a very platform-specific thing. On different platforms, there are different screen readers that might have wildly different output.
    /// Blink - Chrome's rendering engine - has a concept of "accessibility tree", which is than translated into different platform-specific APIs.
    /// Accessibility namespace gives users access to the Blink Accessibility Tree.
    /// Most of the accessibility tree gets filtered out when converting from Blink AX Tree to Platform-specific AX-Tree or by assistive technologies themselves.
    /// By default, Puppeteer tries to approximate this filtering, exposing only the "interesting" nodes of the tree.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1724:Type names should not match namespaces", Justification = "Matches Puppeteer naming.")]
    public class Accessibility
    {
        private readonly DevToolsConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageAccessibility.Accessibility"/> class.
        /// </summary>
        /// <param name="connection">connection.</param>
        public Accessibility(DevToolsConnection connection) => _connection = connection;

        /// <summary>
        /// Snapshots the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="options">Options.</param>
        public async Task<SerializedAXNode> SnapshotAsync(AccessibilitySnapshotOptions options = null)
        {
            var response = await _connection.SendAsync<AccessibilityGetFullAXTreeResponse>("Accessibility.getFullAXTree").ConfigureAwait(false);
            var nodes = response.Nodes;
            object backendNodeId = null;
            if (options?.Root != null)
            {
                var node = await _connection.SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new DomDescribeNodeRequest
                {
                    ObjectId = options.Root.RemoteObject.ObjectId
                }).ConfigureAwait(false);
                backendNodeId = node.Node.BackendNodeId;
            }
            var defaultRoot = AXNode.CreateTree(nodes);
            var needle = defaultRoot;
            if (backendNodeId != null)
            {
                needle = defaultRoot.Find(node => node.Payload.BackendDOMNodeId.Equals(backendNodeId));
                if (needle == null)
                {
                    return null;
                }
            }

            if (options?.InterestingOnly == false)
            {
                return SerializeTree(needle)[0];
            }

            var interestingNodes = new List<AXNode>();
            CollectInterestingNodes(interestingNodes, defaultRoot, false);
            if (!interestingNodes.Contains(needle))
            {
                return null;
            }
            return SerializeTree(needle, interestingNodes)[0];
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
