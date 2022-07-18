using System;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="IDevToolsContext.WaitForRequestAsync(Func{Request, bool}, WaitForOptions)"/>
    /// <seealso cref="IDevToolsContext.WaitForRequestAsync(string, WaitForOptions)"/>
    /// <seealso cref="IDevToolsContext.WaitForResponseAsync(string, WaitForOptions)"/>
    /// <seealso cref="IDevToolsContext.WaitForResponseAsync(Func{Response, bool}, WaitForOptions)"/>
    public class WaitForOptions
    {
        /// <summary>
        /// Maximum time to wait for in milliseconds. Pass 0 to disable timeout.
        /// The default value can be changed by setting the <see cref="IDevToolsContext.DefaultTimeout"/> property.
        /// </summary>
        public int? Timeout { get; set; }
    }
}
