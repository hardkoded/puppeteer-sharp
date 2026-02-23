using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Locators
{
    /// <summary>
    /// A locator that delegates to another locator.
    /// </summary>
    internal abstract class DelegatedLocator : Locator
    {
        private readonly Locator _delegate;

        protected DelegatedLocator(Locator @delegate)
        {
            _delegate = @delegate;
            CopyOptions(_delegate);
        }

        /// <summary>
        /// Gets the delegate locator.
        /// </summary>
        protected Locator Delegate => _delegate;
    }
}
