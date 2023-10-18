using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;
using static PuppeteerSharp.Messaging.AccessibilityGetFullAXTreeResponse;

namespace PuppeteerSharp.QueryHandlers
{
    internal class AriaQueryHandler : QueryHandler
    {
        private static readonly Regex _ariaSelectorAttributeRegEx = new(
            "\\[\\s*(?<attribute>\\w+)\\s*=\\s*(?<quote>\"|')(?<value>\\\\.|.*?(?=\\k<quote>))\\k<quote>\\s*\\]",
            RegexOptions.Compiled);

        private static readonly Regex _normalizedRegex = new(" +", RegexOptions.Compiled);

        public AriaQueryHandler()
        {
            QuerySelector = @"async (node, selector) => {
                const context = globalThis;
                return context.__ariaQuerySelector(node, selector);
            }";
        }

        internal override async IAsyncEnumerable<IElementHandle> QueryAllAsync(IElementHandle element, string selector)
        {
            var context = (ExecutionContext)element.ExecutionContext;
            var world = context.World;
            var ariaSelector = ParseAriaSelector(selector);
            var results = await QueryAXTreeAsync(context.Client, element, ariaSelector.Name, ariaSelector.Role).ConfigureAwait(false);

            foreach (var item in results)
            {
                yield return await world.AdoptBackendNodeAsync(item.BackendDOMNodeId).ConfigureAwait(false);
            }
        }

        internal override async Task<IElementHandle> QueryOneAsync(IElementHandle element, string selector)
        {
            var enumerator = QueryAllAsync(element, selector).GetAsyncEnumerator();
            return await enumerator.MoveNextAsync().ConfigureAwait(false) ? enumerator.Current : default;
        }

        internal override async Task<IElementHandle> WaitForAsync(
            Frame frame,
            ElementHandle element,
            string selector,
            WaitForSelectorOptions options,
            PageBinding[] bindings = null)
        {
            var binding = new PageBinding()
            {
                Name = "__ariaQuerySelector",
                Function = (Func<IElementHandle, string, Task<IElementHandle>>)QueryOneAsync,
            };

            return await base.WaitForAsync(
                frame,
                element,
                selector,
                options,
                new[] { binding }).ConfigureAwait(false);
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

            return nodes.Nodes.Where((node) => node?.Role?.Value?.ToObject<string>() != "StaticText");
        }

        private static AriaQueryOption ParseAriaSelector(string selector)
        {
            static string NormalizeValue(string value) => _normalizedRegex.Replace(value, " ").Trim();

            var knownAriaAttributes = new[] { "name", "role" };
            AriaQueryOption queryOptions = new();
            var defaultName = _ariaSelectorAttributeRegEx.Replace(selector, new MatchEvaluator((Match match) =>
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
            }));

            if (!string.IsNullOrEmpty(defaultName) && string.IsNullOrEmpty(queryOptions.Name))
            {
                queryOptions.Name = NormalizeValue(defaultName);
            }

            return queryOptions;
        }
    }
}
