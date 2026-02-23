using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Locators
{
    /// <summary>
    /// A locator that finds elements using a CSS selector.
    /// </summary>
    internal class NodeLocator : Locator
    {
        private readonly IPage _page;
        private readonly IFrame _frame;
        private readonly string _selector;

        private NodeLocator(IPage page, string selector)
        {
            _page = page;
            _selector = selector;
            Timeout = page.DefaultTimeout;
        }

        private NodeLocator(IFrame frame, string selector)
        {
            _frame = frame;
            _selector = selector;
        }

        internal static Locator Create(IPage page, string selector)
        {
            return new NodeLocator(page, selector);
        }

        internal static Locator Create(IFrame frame, string selector)
        {
            return new NodeLocator(frame, selector);
        }

        internal override async Task<IJSHandle> WaitHandleAsync(LocatorActionOptions options, CancellationToken cancellationToken)
        {
            var waitOptions = new WaitForSelectorOptions
            {
                Timeout = Timeout,
                Visible = Visibility == VisibilityOption.Visible ? true : Visibility == VisibilityOption.Hidden ? false : null,
            };

            IElementHandle handle;

            if (_page != null)
            {
                handle = await _page.WaitForSelectorAsync(_selector, waitOptions).ConfigureAwait(false);
            }
            else
            {
                handle = await _frame.WaitForSelectorAsync(_selector, waitOptions).ConfigureAwait(false);
            }

            if (handle == null)
            {
                throw new PuppeteerException($"Could not find element matching selector: {_selector}");
            }

            return handle;
        }
    }
}
