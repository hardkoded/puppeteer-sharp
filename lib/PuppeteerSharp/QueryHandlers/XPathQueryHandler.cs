namespace PuppeteerSharp.QueryHandlers
{
    internal class XPathQueryHandler : QueryHandler
    {
        internal XPathQueryHandler()
        {
            QuerySelectorAll = @"(element, selector, {xpathQuerySelectorAll}) => {
                return xpathQuerySelectorAll(element, selector);
            }";
        }
    }
}
