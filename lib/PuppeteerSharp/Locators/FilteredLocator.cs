using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Locators
{
    /// <summary>
    /// A locator that filters elements based on a JavaScript predicate.
    /// The predicate is evaluated in the page context. If the predicate does not
    /// match, the locator will retry until it matches or the timeout is reached.
    /// </summary>
    internal class FilteredLocator : DelegatedLocator
    {
        private readonly string _predicate;

        internal FilteredLocator(Locator @base, string predicate)
            : base(@base)
        {
            _predicate = predicate;
        }

        internal override async Task<IJSHandle> WaitHandleCoreAsync(LocatorActionOptions options, CancellationToken cancellationToken)
        {
            var handle = await Delegate.WaitHandleCoreAsync(options, cancellationToken).ConfigureAwait(false);

            var elementHandle = handle as IElementHandle;
            if (elementHandle == null)
            {
                await handle.DisposeAsync().ConfigureAwait(false);
                throw new PuppeteerException("FilteredLocator: handle is not an element.");
            }

            var frame = elementHandle.Frame;
            await frame.WaitForFunctionAsync(
                _predicate,
                new WaitForFunctionOptions { Timeout = Timeout },
                handle).ConfigureAwait(false);

            return handle;
        }
    }
}
