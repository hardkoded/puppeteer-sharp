using System;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Locators
{
    /// <summary>
    /// A locator that filters elements using a handle-based predicate function.
    /// </summary>
    internal class HandleFilteredLocator : DelegatedLocator
    {
        private readonly Func<IJSHandle, Task<bool>> _predicate;

        internal HandleFilteredLocator(Locator @base, Func<IJSHandle, Task<bool>> predicate)
            : base(@base)
        {
            _predicate = predicate;
        }

        internal override async Task<IJSHandle> WaitHandleCoreAsync(LocatorActionOptions options, CancellationToken cancellationToken)
        {
            var handle = await Delegate.WaitHandleCoreAsync(options, cancellationToken).ConfigureAwait(false);

            var matches = await _predicate(handle).ConfigureAwait(false);

            if (!matches)
            {
                await handle.DisposeAsync().ConfigureAwait(false);
                throw new PuppeteerException("HandleFilteredLocator: predicate did not match.");
            }

            return handle;
        }
    }
}
