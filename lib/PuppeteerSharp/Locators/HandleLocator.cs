using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Locators
{
    /// <summary>
    /// A locator that wraps an existing <see cref="IElementHandle"/>.
    /// </summary>
    internal class HandleLocator : Locator
    {
        private readonly IElementHandle _handle;

        internal HandleLocator(IElementHandle handle)
        {
            _handle = handle;
        }

        internal override Task<IJSHandle> WaitHandleCoreAsync(LocatorActionOptions options, CancellationToken cancellationToken)
        {
            return Task.FromResult<IJSHandle>(_handle);
        }
    }
}
