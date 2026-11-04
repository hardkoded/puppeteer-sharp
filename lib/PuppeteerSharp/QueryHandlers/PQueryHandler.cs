namespace PuppeteerSharp.QueryHandlers
{
    internal class PQueryHandler : QueryHandler
    {
        internal PQueryHandler()
        {
            QuerySelector = @"(element, selector, {pQuerySelector}) => {
                return pQuerySelector(element, selector);
            }";

            QuerySelectorAll = @"(element, selector, {pQuerySelectorAll}) => {
                return pQuerySelectorAll(element, selector);
            }";
        }
    }
}
