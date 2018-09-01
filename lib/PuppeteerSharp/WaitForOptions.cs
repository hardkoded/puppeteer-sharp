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
        /// Maximum time to wait for in milliseconds. Defaults to 30000 (30 seconds). Pass 0 to disable timeout.
        /// </summary>
        public int Timeout { get; set; } = 30_000;
    }
}