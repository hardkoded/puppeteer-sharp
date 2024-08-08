namespace PuppeteerSharp.QueryHandlers
{
    internal class CssQueryHandler : QueryHandler
    {
        internal CssQueryHandler()
        {
            QuerySelector = @"(element, selector) => {
                return element.querySelector(selector);
            }";

            QuerySelectorAll = @"(element, selector) => {
                return element.querySelectorAll(selector);
            }";
        }
    }
}
