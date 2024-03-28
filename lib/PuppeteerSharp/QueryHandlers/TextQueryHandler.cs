using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PuppeteerSharp.Cdp.Messaging.AccessibilityGetFullAXTreeResponse;

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
