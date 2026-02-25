using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.PageAccessibility
{
    /// <inheritdoc/>
    public class Accessibility : IAccessibility
    {
        private readonly Func<string> _getFrameId;
        private readonly Func<Realm> _realmProvider;
        private CDPSession _client;

        internal Accessibility(CDPSession client, Func<string> getFrameId, Func<Realm> realmProvider)
        {
            _client = client;
            _getFrameId = getFrameId;
            _realmProvider = realmProvider;
        }

        /// <inheritdoc/>
        public async Task<SerializedAXNode> SnapshotAsync(AccessibilitySnapshotOptions options = null)
        {
            var response = await _client.SendAsync<AccessibilityGetFullAXTreeResponse>(
                "Accessibility.getFullAXTree",
                new AccessibilityGetFullAXTreeRequest { FrameId = _getFrameId() }).ConfigureAwait(false);
            var nodes = response.Nodes;
            JsonElement? backendNodeId = null;
            if (options?.Root != null)
            {
                var node = await _client.SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new DomDescribeNodeRequest
                {
                    ObjectId = ((CdpElementHandle)options.Root).RemoteObject.ObjectId,
                }).ConfigureAwait(false);
                backendNodeId = node.Node.BackendNodeId;
            }

            var realm = _realmProvider?.Invoke();
            var defaultRoot = AXNode.CreateTree(realm, nodes);
            if (defaultRoot == null)
            {
                return null;
            }

            if (options?.IncludeIframes == true)
            {
                await PopulateIframesAsync(defaultRoot, options).ConfigureAwait(false);
            }

            var needle = defaultRoot;
            if (backendNodeId != null)
            {
                needle = defaultRoot.Find(node =>
                    node.Payload.BackendDOMNodeId.ValueKind == JsonValueKind.Number &&
                    node.Payload.BackendDOMNodeId.GetInt32() == backendNodeId.Value.GetInt32());
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

            var result = SerializeTree(needle, interestingNodes);
            return result.Length > 0 ? result[0] : null;
        }

        internal void UpdateClient(CDPSession client) => _client = client;

        private async Task PopulateIframesAsync(AXNode root, AccessibilitySnapshotOptions options)
        {
            var realm = _realmProvider?.Invoke();

            if (root.Payload.Role?.Value.ToObject<string>() == "Iframe")
            {
                if (root.Payload.BackendDOMNodeId.ValueKind != JsonValueKind.Number || realm == null)
                {
                    return;
                }

                try
                {
                    var handle = await realm.AdoptBackendNodeAsync(root.Payload.BackendDOMNodeId.GetInt32()).ConfigureAwait(false);

                    if (handle is not IElementHandle elementHandle)
                    {
                        return;
                    }

                    var frame = await elementHandle.ContentFrameAsync().ConfigureAwait(false);

                    if (frame is not CdpFrame cdpFrame)
                    {
                        return;
                    }

                    var iframeSnapshot = await cdpFrame.Accessibility.SnapshotAsync(options).ConfigureAwait(false);
                    root.IframeSnapshot = iframeSnapshot;
                }
                catch (Exception ex)
                {
                    // Frames can get detached at any time resulting in errors.
                    var logger = _client.Connection?.LoggerFactory?.CreateLogger<Accessibility>();
                    logger?.LogError(ex, "Error getting iframe accessibility snapshot");
                }
            }

            foreach (var child in root.Children)
            {
                await PopulateIframesAsync(child, options).ConfigureAwait(false);
            }
        }

        private void CollectInterestingNodes(List<AXNode> collection, AXNode node, bool insideControl)
        {
            if (node.IsInteresting(insideControl) || node.IframeSnapshot != null)
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

            if (node.IframeSnapshot != null)
            {
                serializedNode.Children ??= [];
                var childrenList = new List<SerializedAXNode>(serializedNode.Children)
                {
                    node.IframeSnapshot,
                };
                serializedNode.Children = [.. childrenList];
            }

            return [serializedNode];
        }
    }
}
