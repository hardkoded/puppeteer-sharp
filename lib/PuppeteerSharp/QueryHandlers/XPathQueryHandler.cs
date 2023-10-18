using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;
using static PuppeteerSharp.Messaging.AccessibilityGetFullAXTreeResponse;

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
