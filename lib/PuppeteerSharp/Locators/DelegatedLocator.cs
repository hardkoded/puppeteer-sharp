using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Locators
{
    /// <summary>
    /// A locator that delegates to another locator.
    /// </summary>
    internal abstract class DelegatedLocator : Locator
    {
        private Locator _delegate;

        protected DelegatedLocator(Locator @delegate)
        {
            _delegate = @delegate;
            CopyOptions(_delegate);
        }

        /// <summary>
        /// Gets the delegate locator.
        /// </summary>
        protected Locator Delegate => _delegate;

        /// <inheritdoc />
        public override Locator SetTimeout(int timeout)
        {
            var locator = (DelegatedLocator)base.SetTimeout(timeout);
            locator._delegate = _delegate.SetTimeout(timeout);
            return locator;
        }

        /// <inheritdoc />
        public override Locator SetVisibility(VisibilityOption? visibility)
        {
            var locator = (DelegatedLocator)base.SetVisibility(visibility);
            locator._delegate = locator._delegate.SetVisibility(visibility);
            return locator;
        }

        /// <inheritdoc />
        public override Locator SetWaitForEnabled(bool value)
        {
            var locator = (DelegatedLocator)base.SetWaitForEnabled(value);
            locator._delegate = _delegate.SetWaitForEnabled(value);
            return locator;
        }

        /// <inheritdoc />
        public override Locator SetEnsureElementIsInTheViewport(bool value)
        {
            var locator = (DelegatedLocator)base.SetEnsureElementIsInTheViewport(value);
            locator._delegate = _delegate.SetEnsureElementIsInTheViewport(value);
            return locator;
        }

        /// <inheritdoc />
        public override Locator SetWaitForStableBoundingBox(bool value)
        {
            var locator = (DelegatedLocator)base.SetWaitForStableBoundingBox(value);
            locator._delegate = _delegate.SetWaitForStableBoundingBox(value);
            return locator;
        }
    }
}
