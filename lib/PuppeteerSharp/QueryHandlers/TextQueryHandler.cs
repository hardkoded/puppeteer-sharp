namespace PuppeteerSharp.QueryHandlers
{
    internal class TextQueryHandler : QueryHandler
    {
        internal TextQueryHandler()
        {
            QuerySelectorAll = @"(element, selector, {textQuerySelectorAll}) => {
                return textQuerySelectorAll(element, selector);
            }";
        }
    }
}
