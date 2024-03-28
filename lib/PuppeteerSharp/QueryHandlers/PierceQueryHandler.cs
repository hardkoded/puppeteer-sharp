using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PuppeteerSharp.Cdp.Messaging.AccessibilityGetFullAXTreeResponse;

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
