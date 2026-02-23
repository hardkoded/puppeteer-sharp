using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Locators
{
    /// <summary>
    /// A locator that maps the located element using a JavaScript function.
    /// </summary>
    internal class MappedLocator : DelegatedLocator
    {
        private readonly string _mapper;

        internal MappedLocator(Locator @base, string mapper)
            : base(@base)
        {
            _mapper = mapper;
        }

        internal override async Task<IJSHandle> WaitHandleAsync(LocatorActionOptions options, CancellationToken cancellationToken)
        {
            var handle = await Delegate.WaitHandleAsync(options, cancellationToken).ConfigureAwait(false);

            try
            {
                return await handle.EvaluateFunctionHandleAsync(_mapper).ConfigureAwait(false);
            }
            finally
            {
                await handle.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
