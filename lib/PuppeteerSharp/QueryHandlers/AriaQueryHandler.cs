using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;
using static PuppeteerSharp.Cdp.Messaging.AccessibilityGetFullAXTreeResponse;

namespace PuppeteerSharp.QueryHandlers
{
    internal class AriaQueryHandler : QueryHandler
    {
        private static readonly Regex _ariaSelectorAttributeRegEx = new(
            "\\[\\s*(?<attribute>\\w+)\\s*=\\s*(?<quote>\"|')(?<value>\\\\.|.*?(?=\\k<quote>))\\k<quote>\\s*\\]",
            RegexOptions.Compiled);

        private static readonly Regex _normalizedRegex = new(" +", RegexOptions.Compiled);
        private static readonly string[] _nonElementNodeRoles = { "StaticText", "InlineTextBox" };

        public AriaQueryHandler()
        {
            QuerySelector = @"async (node, selector) => {
                const context = globalThis;
                return context.__ariaQuerySelector(node, selector);
            }";
        }

        internal override async IAsyncEnumerable<IElementHandle> QueryAllAsync(IElementHandle element, string selector)
        {
            var ariaSelector = ParseAriaSelector(selector);
            if (element is not ElementHandle elementHandle)
            {
                yield break;
            }

            var results = await QueryAXTreeAsync(elementHandle.Realm.Environment.Client, element, ariaSelector.Name, ariaSelector.Role).ConfigureAwait(false);

            foreach (var item in results)
            {
                yield return await elementHandle.Realm.AdoptBackendNodeAsync(item.BackendDOMNodeId).ConfigureAwait(false);
            }
        }

        internal override async Task<IElementHandle> QueryOneAsync(IElementHandle element, string selector)
        {
            var enumerator = QueryAllAsync(element, selector).GetAsyncEnumerator();
            return await enumerator.MoveNextAsync().ConfigureAwait(false) ? enumerator.Current : default;
        }

        private static async Task<IEnumerable<AXTreeNode>> QueryAXTreeAsync(CDPSession client, IElementHandle element, string accessibleName, string role)
        {
            var nodes = await client.SendAsync<AccessibilityQueryAXTreeResponse>(
                "Accessibility.queryAXTree",
                new AccessibilityQueryAXTreeRequest()
                {
                    ObjectId = element.RemoteObject.ObjectId,
                    AccessibleName = accessibleName,
                    Role = role,
                }).ConfigureAwait(false);

            return nodes.Nodes.Where(node =>
                node?.Role?.Value?.ToObject<string>() is not null &&
                !_nonElementNodeRoles.Contains(node.Role.Value.ToObject<string>()));
        }

        private static AriaQueryOption ParseAriaSelector(string selector)
        {
            var knownAriaAttributes = new[] { "name", "role" };
            AriaQueryOption queryOptions = new();
            var defaultName = _ariaSelectorAttributeRegEx.Replace(selector, match =>
            {
                var attribute = match.Groups["attribute"].Value.Trim();
                if (!knownAriaAttributes.Contains(attribute))
                {
                    throw new PuppeteerException($"Unknown aria attribute \"{attribute}\" in selector");
                }

                if (attribute == "name")
                {
                    queryOptions.Name = NormalizeValue(match.Groups["value"].Value);
                }
                else
                {
                    queryOptions.Role = NormalizeValue(match.Groups["value"].Value);
                }

                return string.Empty;
            });

            if (!string.IsNullOrEmpty(defaultName) && string.IsNullOrEmpty(queryOptions.Name))
            {
                queryOptions.Name = NormalizeValue(defaultName);
            }

            return queryOptions;

            static string NormalizeValue(string value) => _normalizedRegex.Replace(value, " ").Trim();
        }
    }
}
