using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PuppeteerSharp.Cdp.Messaging.AccessibilityGetFullAXTreeResponse;

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
