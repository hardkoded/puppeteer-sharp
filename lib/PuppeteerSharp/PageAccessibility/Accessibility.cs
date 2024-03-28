using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.PageAccessibility
{
    /// <inheritdoc/>
    public class Accessibility : IAccessibility
    {
        private CDPSession _client;

        /// <inheritdoc cref="Accessibility"/>
        public Accessibility(CDPSession client) => _client = client;

        /// <inheritdoc/>
        public async Task<SerializedAXNode> SnapshotAsync(AccessibilitySnapshotOptions options = null)
        {
            var response = await _client.SendAsync<AccessibilityGetFullAXTreeResponse>("Accessibility.getFullAXTree").ConfigureAwait(false);
            var nodes = response.Nodes;
            object backendNodeId = null;
            if (options?.Root != null)
            {
                var node = await _client.SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new DomDescribeNodeRequest
                {
                    ObjectId = options.Root.RemoteObject.ObjectId,
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

        internal void UpdateClient(CDPSession client) => _client = client;

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
                serializedNode.Children = [.. children];
            }

            return [serializedNode];
        }
    }
}
