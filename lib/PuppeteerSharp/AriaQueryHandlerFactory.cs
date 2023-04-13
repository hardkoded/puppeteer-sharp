using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;
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
            async Task<object> QueryOneId(IElementHandle element, string selector)
            {
                var queryOptions = ParseAriaSelector(selector);
                var res = await QueryAXTreeAsync(((ElementHandle)element).Client, element, queryOptions.Name, queryOptions.Role).ConfigureAwait(false);

                return res.FirstOrDefault()?.BackendDOMNodeId;
            }

            async Task<IElementHandle> QueryOne(IElementHandle element, string selector)
            {
                var id = await QueryOneId(element, selector).ConfigureAwait(false);
                if (id == null)
                {
                    return null;
                }

                return await ((ElementHandle)element).Frame.SecondaryWorld.AdoptBackendNodeAsync(id).ConfigureAwait(false);
            }

            async Task<IElementHandle> WaitFor(IElementHandle root, string selector, WaitForSelectorOptions options)
            {
                var frame = ((ElementHandle)root).Frame;
                var element = (await frame.SecondaryWorld.AdoptHandleAsync(root).ConfigureAwait(false)) as IElementHandle;

                Task<IElementHandle> Func(string selector) => QueryOne(element, selector);

                var binding = new PageBinding()
                {
                    Name = "ariaQuerySelector",
                    Function = (Func<string, Task<IElementHandle>>)Func,
                };

                return await frame.SecondaryWorld.WaitForSelectorInPageAsync(
                    @"(_, selector) => globalThis.ariaQuerySelector(selector)",
                    selector,
                    options,
                    new[] { binding }).ConfigureAwait(false);
            }

            async Task<IElementHandle[]> QueryAll(IElementHandle element, string selector)
            {
                var exeCtx = (ExecutionContext)element.ExecutionContext;
                var world = exeCtx.World;
                var ariaSelector = ParseAriaSelector(selector);
                var res = await QueryAXTreeAsync(exeCtx.Client, element, ariaSelector.Name, ariaSelector.Role).ConfigureAwait(false);
                var elements = await Task.WhenAll(res.Select(axNode => world.AdoptBackendNodeAsync(axNode.BackendDOMNodeId))).ConfigureAwait(false);
                return elements.ToArray();
            }

            async Task<IJSHandle> QueryAllArray(IElementHandle element, string selector)
            {
                var elementHandles = await QueryAll(element, selector).ConfigureAwait(false);
                var exeCtx = element.ExecutionContext;
                var jsHandle = await exeCtx.EvaluateFunctionHandleAsync("(...elements) => elements", elementHandles).ConfigureAwait(false);
                return jsHandle;
            }

            return new()
            {
                QueryOne = QueryOne,
                WaitFor = WaitFor,
                QueryAll = QueryAll,
                QueryAllArray = QueryAllArray,
            };
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
