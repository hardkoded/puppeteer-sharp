using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp.QueryHandlers
{
    internal class QueryHandler
    {
        private string _querySelector;
        private string _querySelectorAll;

        public string QuerySelectorAll
        {
            get
            {
                if (!string.IsNullOrEmpty(_querySelectorAll))
                {
                    return _querySelectorAll;
                }

                var querySelectorAll = @"async (node, selector, PuppeteerUtil) => {
                    const querySelectorAll = 'FUNCTION_DEFINITION';
                    const result = await querySelector(node, selector, PuppeteerUtil);
                    if (result) {
                        yield result;
                    }
                }";

                // Upstream uses a CreateFunction util. We don't need that because we always use strings instead of node functions.
                _querySelectorAll = querySelectorAll.Replace("'FUNCTION_DEFINITION'", QuerySelector);
                return _querySelectorAll;
            }

            set
            {
                _querySelectorAll = value;
            }
        }

        internal string QuerySelector
        {
            get
            {
                if (!string.IsNullOrEmpty(_querySelector))
                {
                    return _querySelector;
                }

                if (string.IsNullOrEmpty(QuerySelectorAll))
                {
                    throw new PuppeteerException("Cannot create default query selector");
                }

                var querySelector = @"async (node, selector, PuppeteerUtil) => {
                    const querySelectorAll = 'FUNCTION_DEFINITION';
                    const results = querySelectorAll(node, selector, PuppeteerUtil);
                    for await (const result of results) {
                        return result;
                    }
                    return null;
                }";

                // Upstream uses a CreateFunction util. We don't need that because we always use strings instead of node functions.
                _querySelector = querySelector.Replace("'FUNCTION_DEFINITION'", QuerySelectorAll);
                return _querySelector;
            }

            set
            {
                _querySelector = value;
            }
        }

        internal virtual async Task<IElementHandle> QueryOneAsync(IElementHandle element, string selector)
        {
            var result = await element.EvaluateFunctionHandleAsync(
                QuerySelector,
                selector,
                new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)))
                .ConfigureAwait(false);

            if (result is ElementHandle elementHandle)
            {
                return elementHandle;
            }

            await result.DisposeAsync().ConfigureAwait(false);
            return null;
        }

        internal async Task<IElementHandle> WaitForAsync(
            Frame frame,
            ElementHandle element,
            string selector,
            WaitForSelectorOptions options)
        {
            if (element != null)
            {
                frame = element.Frame;
                element = await frame.IsolatedRealm.AdoptHandleAsync(element).ConfigureAwait(false) as ElementHandle;
            }

            try
            {
                var waitForVisible = options?.Visible ?? false;
                var waitForHidden = options?.Hidden ?? false;
                var timeout = options?.Timeout;

                var predicate = @$"async (PuppeteerUtil, query, selector, root, visible) => {{
                  if (!PuppeteerUtil) {{
                    return;
                  }}
                  const node = (await PuppeteerUtil.createFunction(query)(
                    root || document,
                    selector,
                    PuppeteerUtil,
                  ));
                  return PuppeteerUtil.checkVisibility(node, visible);
                }}";

                var args = new List<object>
                {
                    new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
                    QuerySelector,
                    selector,
                    element,
                };

                // Puppeteer's injected code checks for visible to be undefined
                // As we don't support passing undefined values we need to ignore sending this value
                // if visible is false
                if (waitForVisible || waitForHidden)
                {
                    args.Add(waitForVisible);
                }

                var jsHandle = await frame.IsolatedRealm.WaitForFunctionAsync(
                    predicate,
                    new()
                    {
                        Polling = waitForVisible || waitForHidden ? WaitForFunctionPollingOption.Raf : WaitForFunctionPollingOption.Mutation,
                        Root = element,
                        Timeout = timeout,
                    },
                    args.ToArray()).ConfigureAwait(false);

                if (jsHandle is not ElementHandle elementHandle)
                {
                    await jsHandle.DisposeAsync().ConfigureAwait(false);
                    return null;
                }

                return await frame.MainRealm.TransferHandleAsync(elementHandle).ConfigureAwait(false) as IElementHandle;
            }
            catch (Exception ex)
            {
                throw new WaitTaskTimeoutException($"Waiting for selector `{selector}` failed: {ex.Message}", ex);
            }
        }

        internal virtual async IAsyncEnumerable<IElementHandle> QueryAllAsync(IElementHandle element, string selector)
        {
            var handle = await element.EvaluateFunctionHandleAsync(
                QuerySelectorAll,
                selector,
                new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)))
                .ConfigureAwait(false);

            await foreach (var item in handle.TransposeIterableHandleAsync())
            {
                yield return item;
            }
        }
    }
}
