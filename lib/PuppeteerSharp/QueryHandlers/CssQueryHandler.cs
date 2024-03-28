using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PuppeteerSharp.Cdp.Messaging.AccessibilityGetFullAXTreeResponse;

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
