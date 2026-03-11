using System;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Locators
{
    /// <summary>
    /// A locator that maps the located element using a handle-based mapper function.
    /// </summary>
    internal class HandleMappedLocator : DelegatedLocator
    {
        private readonly Func<IJSHandle, Task<IJSHandle>> _mapper;

        internal HandleMappedLocator(Locator @base, Func<IJSHandle, Task<IJSHandle>> mapper)
            : base(@base)
        {
            _mapper = mapper;
        }

        internal override async Task<IJSHandle> WaitHandleCoreAsync(LocatorActionOptions options, CancellationToken cancellationToken)
        {
            var handle = await Delegate.WaitHandleCoreAsync(options, cancellationToken).ConfigureAwait(false);

            try
            {
                return await _mapper(handle).ConfigureAwait(false);
            }
            finally
            {
                await handle.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
