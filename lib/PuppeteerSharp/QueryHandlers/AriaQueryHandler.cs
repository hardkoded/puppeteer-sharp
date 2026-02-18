using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#if !CDP_ONLY
using PuppeteerSharp.Bidi;
#endif
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers.Json;
using static PuppeteerSharp.Cdp.Messaging.AccessibilityGetFullAXTreeResponse;

namespace PuppeteerSharp.QueryHandlers
{
    internal partial class AriaQueryHandler : QueryHandler
    {
#if NETSTANDARD2_0
        private static readonly Regex _ariaSelectorAttributeRegex = new(
            """\[\s*(?<attribute>\w+)\s*=\s*(?<quote>"|')(?<value>\\.|.*?(?=\k<quote>))\k<quote>\s*\]""",
            RegexOptions.Compiled);
#endif

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

#if !CDP_ONLY
            // Handle BiDi element handles
            if (element is BidiElementHandle bidiElementHandle)
            {
                await foreach (var item in bidiElementHandle.QueryAXTreeAsync(ariaSelector.Name, ariaSelector.Role).ConfigureAwait(false))
                {
                    yield return item;
                }

                yield break;
            }
#endif

            // Handle CDP element handles
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

        internal override async Task<IElementHandle> WaitForAsync(
            Frame frame,
            ElementHandle element,
            string selector,
            WaitForSelectorOptions options)
        {
            // Get frame from element if not provided
            var targetFrame = frame ?? element?.Frame;

#if !CDP_ONLY
            // For BiDi frames, use native BiDi approach with polling
            if (targetFrame is BidiFrame)
            {
                return await WaitForBidiAsync(targetFrame, element, selector, options).ConfigureAwait(false);
            }
#endif

            // For CDP frames, use the base implementation
            return await base.WaitForAsync(frame, element, selector, options).ConfigureAwait(false);
        }

#if NET8_0_OR_GREATER
        [GeneratedRegex("""\[\s*(?<attribute>\w+)\s*=\s*(?<quote>"|')(?<value>\\.|.*?(?=\k<quote>))\k<quote>\s*\]""")]
        private static partial Regex GetAriaSelectorAttributeRegex();
#else
        private static Regex GetAriaSelectorAttributeRegex() => _ariaSelectorAttributeRegex;
#endif

#if !CDP_ONLY
        private static async Task<bool> IsVisibleAsync(IElementHandle element)
        {
            return await element.EvaluateFunctionAsync<bool>(@"(element) => {
                const style = window.getComputedStyle(element);
                return style && style.visibility !== 'hidden' && style.display !== 'none';
            }").ConfigureAwait(false);
        }
#endif

        private static async Task<IEnumerable<AXTreeNode>> QueryAXTreeAsync(ICDPSession client, IElementHandle element, string accessibleName, string role)
        {
            var cdpElementHandle = (CdpElementHandle)element;
            var nodes = await client.SendAsync<AccessibilityQueryAXTreeResponse>(
                "Accessibility.queryAXTree",
                new AccessibilityQueryAXTreeRequest()
                {
                    ObjectId = cdpElementHandle.RemoteObject.ObjectId,
                    AccessibleName = accessibleName,
                    Role = role,
                }).ConfigureAwait(false);

            return nodes.Nodes.Where(node =>
            {
                if (node.Ignored)
                {
                    return false;
                }

                if (node.Role == null)
                {
                    return false;
                }

                return !_nonElementNodeRoles.Contains(node.Role.Value.ToObject<string>());
            });
        }

        private static AriaQueryOption ParseAriaSelector(string selector)
        {
            var knownAriaAttributes = new[] { "name", "role" };
            AriaQueryOption queryOptions = new();
            var defaultName = GetAriaSelectorAttributeRegex().Replace(selector, match =>
            {
                var attribute = match.Groups["attribute"].Value;
                if (!knownAriaAttributes.Contains(attribute))
                {
                    throw new PuppeteerException($"Unknown aria attribute \"{attribute}\" in selector");
                }

                if (attribute == "name")
                {
                    queryOptions.Name = match.Groups["value"].Value;
                }
                else
                {
                    queryOptions.Role = match.Groups["value"].Value;
                }

                return string.Empty;
            });

            if (!string.IsNullOrEmpty(defaultName) && string.IsNullOrEmpty(queryOptions.Name))
            {
                queryOptions.Name = defaultName;
            }

            return queryOptions;
        }

#if !CDP_ONLY
        private async Task<IElementHandle> WaitForBidiAsync(
            Frame frame,
            ElementHandle element,
            string selector,
            WaitForSelectorOptions options)
        {
            var timeout = options?.Timeout ?? (frame.Page as Page)?.TimeoutSettings.Timeout ?? 30000;
            var waitForVisible = options?.Visible ?? false;
            var waitForHidden = options?.Hidden ?? false;

            var sw = Stopwatch.StartNew();

            try
            {
                while (true)
                {
                    if (sw.ElapsedMilliseconds > timeout)
                    {
                        throw new TimeoutException($"Waiting for selector `{selector}` failed: timeout {timeout}ms exceeded");
                    }

                    IElementHandle queryElement = element;
                    if (queryElement == null)
                    {
                        // Get the document element as the root
                        queryElement = await frame.QuerySelectorAsync("body").ConfigureAwait(false) as ElementHandle;
                        if (queryElement == null)
                        {
                            // Fallback to html element
                            queryElement = await frame.QuerySelectorAsync("html").ConfigureAwait(false) as ElementHandle;
                        }
                    }

                    if (queryElement != null)
                    {
                        var result = await QueryOneAsync(queryElement, selector).ConfigureAwait(false);

                        if (element == null && queryElement != null)
                        {
                            await queryElement.DisposeAsync().ConfigureAwait(false);
                        }

                        if (result != null)
                        {
                            if (waitForVisible)
                            {
                                var isVisible = await IsVisibleAsync(result).ConfigureAwait(false);
                                if (isVisible)
                                {
                                    return result;
                                }
                            }
                            else if (waitForHidden)
                            {
                                var isVisible = await IsVisibleAsync(result).ConfigureAwait(false);
                                if (!isVisible)
                                {
                                    return result;
                                }
                            }
                            else
                            {
                                return result;
                            }

                            await result.DisposeAsync().ConfigureAwait(false);
                        }
                        else if (waitForHidden)
                        {
                            // Element not found and we're waiting for hidden - success
                            return null;
                        }
                    }

                    // Small delay to avoid busy-waiting
                    await Task.Delay(100).ConfigureAwait(false);
                }
            }
            catch (TimeoutException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new WaitTaskTimeoutException($"Waiting for selector `{selector}` failed: {ex.Message}", ex);
            }
        }
#endif

    }
}
