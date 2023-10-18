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
            var world = (element.ExecutionContext as ExecutionContext).World
                ?? throw new PuppeteerException("Element doesn't have a valid world");

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

        internal virtual async Task<IElementHandle> WaitForAsync(
            Frame frame,
            ElementHandle element,
            string selector,
            WaitForSelectorOptions options,
            PageBinding[] bindings = null)
        {
            if (element != null)
            {
                frame = element.Frame as Frame;
                element = await frame.PuppeteerWorld.AdoptHandleAsync(element).ConfigureAwait(false) as ElementHandle;
            }

            var result = await frame.PuppeteerWorld.WaitForSelectorInPageAsync(
                QuerySelector,
                element,
                selector,
                options,
                bindings).ConfigureAwait(false);

            if (element != null)
            {
                await element.DisposeAsync().ConfigureAwait(false);
            }

            if (result is not ElementHandle)
            {
                await element.DisposeAsync().ConfigureAwait(false);
                return null;
            }

            return await frame.MainWorld.TransferHandleAsync(result).ConfigureAwait(false) as IElementHandle;
        }

        internal virtual async IAsyncEnumerable<IElementHandle> QueryAllAsync(IElementHandle element, string selector)
        {
            var world = (element.ExecutionContext as ExecutionContext).World
                ?? throw new PuppeteerException("Element doesn't have a valid world");

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
