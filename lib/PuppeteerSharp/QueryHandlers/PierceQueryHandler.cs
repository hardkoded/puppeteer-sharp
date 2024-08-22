namespace PuppeteerSharp.QueryHandlers
{
    internal class PierceQueryHandler : QueryHandler
    {
        internal PierceQueryHandler()
        {
            QuerySelector = @"(element, selector, {pierceQuerySelector}) => {
                return pierceQuerySelector(element, selector);
            }";

            QuerySelectorAll = @"(element, selector, {pierceQuerySelectorAll}) => {
                return pierceQuerySelectorAll(element, selector);
            }";
        }
    }
}
