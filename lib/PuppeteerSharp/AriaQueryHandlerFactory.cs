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

        internal static QueryHandler Create()
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

                return await ((ElementHandle)element).Frame.PuppeteerWorld.AdoptBackendNodeAsync(id).ConfigureAwait(false);
            }

            async Task<IElementHandle> WaitFor(IFrame frame, IElementHandle element, string selector, WaitForSelectorOptions options)
            {
                var frameImpl = frame as Frame;

                if (element != null)
                {
                    frameImpl = ((ElementHandle)element).Frame;
                    element = (await frameImpl.PuppeteerWorld.AdoptHandleAsync(element).ConfigureAwait(false)) as IElementHandle;
                }

                async Task<IElementHandle> Func(string selector)
                {
                    var id = await QueryOneId(
                        element ?? (await frameImpl.PuppeteerWorld.GetDocumentAsync().ConfigureAwait(false)),
                        selector).ConfigureAwait(false);

                    if (id == null)
                    {
                        return null;
                    }

                    return await frameImpl.PuppeteerWorld.AdoptBackendNodeAsync(id).ConfigureAwait(false);
                }

                var binding = new PageBinding()
                {
                    Name = "ariaQuerySelector",
                    Function = (Func<string, Task<IElementHandle>>)Func,
                };

                var result = await frameImpl.PuppeteerWorld.WaitForSelectorInPageAsync(
                    @"(_, selector) => globalThis.ariaQuerySelector(selector)",
                    element,
                    selector,
                    options,
                    new[] { binding }).ConfigureAwait(false);

                if (element != null)
                {
                    await element.DisposeAsync().ConfigureAwait(false);
                }

                if (result == null)
                {
                    return null;
                }

                return await frameImpl.MainWorld.TransferHandleAsync(result).ConfigureAwait(false) as IElementHandle;
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

            return new()
            {
                QueryOne = QueryOne,
                WaitFor = WaitFor,
                QueryAll = QueryAll,
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
