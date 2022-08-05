using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.PageAccessibility;
using static PuppeteerSharp.Messaging.AccessibilityGetFullAXTreeResponse;

namespace PuppeteerSharp
{
    internal class AriaQueryHandlerFactory
    {
        private static readonly Regex _ariaSelectorAttributeRegEx = new(
            "\\[\\s*(?<attribute>\\w+)\\s*=\\s*(?<quote>\"|')(?<value>\\\\.|.*?(?=\\k<quote>))\\k<quote>\\s*\\]",
            RegexOptions.Compiled);

        private static readonly Regex _normalizedRegex = new(" +", RegexOptions.Compiled);

        internal static InternalQueryHandler Create()
        {
            Func<ElementHandle, string, Task<ElementHandle>> queryOne = async (ElementHandle element, string selector) =>
            {
                var exeCtx = element.ExecutionContext;
                var queryOptions = ParseAriaSelector(selector);
                var res = await QueryAXTreeAsync(exeCtx.Client, element, queryOptions.Name, queryOptions.Role).ConfigureAwait(false);
                if (!res.Any() || res.First().BackendDOMNodeId == null)
                {
                    return null;
                }
                return await exeCtx.AdoptBackendNodeAsync(res.First().BackendDOMNodeId).ConfigureAwait(false);
            };

            Func<DOMWorld, string, WaitForSelectorOptions, Task<ElementHandle>> waitFor = (DOMWorld domWorld, string selector, WaitForSelectorOptions options) =>
            {
                Func<string, Task<ElementHandle>> func = async (string selector) =>
                {
                    var root = options.Root ?? await domWorld.GetDocumentAsync().ConfigureAwait(false);
                    var element = await queryOne(root, selector).ConfigureAwait(false);
                    return element;
                };

                var binding = new PageBinding()
                {
                    Name = "ariaQuerySelector",
                    Function = (Delegate)func,
                };

                return domWorld.WaitForSelectorInPageAsync(
                    @"(_, selector) => return globalThis.ariaQuerySelector(selector)",
                    selector,
                    options,
                    binding);
            };

            return new()
            {
                QueryOne = queryOne,
                WaitFor = waitFor,
            };
        }

        private static async Task<IEnumerable<AXTreeNode>> QueryAXTreeAsync(CDPSession client, ElementHandle element, string accessibleName, string role)
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
            string NormalizeValue(string value) => _normalizedRegex.Replace(value, " ").Trim();

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