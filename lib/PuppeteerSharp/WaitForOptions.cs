using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="Page.WaitForRequestAsync(Func{Request, bool}, WaitForOptions)"/>
    /// <seealso cref="Page.WaitForRequestAsync(string, WaitForOptions)"/>
    /// <seealso cref="Page.WaitForResponseAsync(string, WaitForOptions)"/>
    /// <seealso cref="Page.WaitForResponseAsync(Func{Response, bool}, WaitForOptions)"/>
    public class WaitForOptions
    {
        /// <summary>
        /// Maximum time to wait for in milliseconds. Pass 0 to disable timeout.
        /// The default value can be changed by setting the <see cref="Page.DefaultTimeout"/> property.
        /// </summary>
        public int? Timeout { get; set; }
    }
}
