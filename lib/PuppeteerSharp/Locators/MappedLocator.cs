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
        private readonly object[] _args;

        internal MappedLocator(Locator @base, string mapper, object[] args)
            : base(@base)
        {
            _mapper = mapper;
            _args = args;
        }

        internal override async Task<IJSHandle> WaitHandleCoreAsync(LocatorActionOptions options, CancellationToken cancellationToken)
        {
            var handle = await Delegate.WaitHandleCoreAsync(options, cancellationToken).ConfigureAwait(false);

            try
            {
                return await handle.EvaluateFunctionHandleAsync(_mapper, _args).ConfigureAwait(false);
            }
            finally
            {
                await handle.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
