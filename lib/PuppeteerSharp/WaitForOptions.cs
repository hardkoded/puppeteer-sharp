using System;

namespace CefSharp.Puppeteer
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="DevToolsContext.WaitForRequestAsync(Func{Request, bool}, WaitForOptions)"/>
    /// <seealso cref="DevToolsContext.WaitForRequestAsync(string, WaitForOptions)"/>
    /// <seealso cref="DevToolsContext.WaitForResponseAsync(string, WaitForOptions)"/>
    /// <seealso cref="DevToolsContext.WaitForResponseAsync(Func{Response, bool}, WaitForOptions)"/>
    public class WaitForOptions
    {
        /// <summary>
        /// Maximum time to wait for in milliseconds. Pass 0 to disable timeout.
        /// The default value can be changed by setting the <see cref="DevToolsContext.DefaultTimeout"/> property.
        /// </summary>
        public int? Timeout { get; set; }
    }
}