using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Locators
{
    /// <summary>
    /// A locator that evaluates a JavaScript function and waits for it to return a truthy value.
    /// </summary>
    internal class FunctionLocator : Locator
    {
        private readonly IPage _page;
        private readonly IFrame _frame;
        private readonly string _func;

        private FunctionLocator(IPage page, string func)
        {
            _page = page;
            _func = func;
            Timeout = page.DefaultTimeout;
        }

        private FunctionLocator(IFrame frame, string func)
        {
            _frame = frame;
            _func = func;
        }

        internal static Locator Create(IPage page, string func)
        {
            return new FunctionLocator(page, func);
        }

        internal static Locator Create(IFrame frame, string func)
        {
            return new FunctionLocator(frame, func);
        }

        internal override async Task<IJSHandle> WaitHandleAsync(LocatorActionOptions options, CancellationToken cancellationToken)
        {
            var waitOptions = new WaitForFunctionOptions
            {
                Timeout = Timeout,
            };

            IJSHandle handle;

            if (_page != null)
            {
                handle = await _page.WaitForFunctionAsync(_func, waitOptions).ConfigureAwait(false);
            }
            else
            {
                handle = await _frame.WaitForFunctionAsync(_func, waitOptions).ConfigureAwait(false);
            }

            return handle;
        }
    }
}
