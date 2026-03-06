using System;
using System.Collections.Generic;

namespace PuppeteerSharp
{
    /// <summary>
    /// Debug information about the browser.
    /// Currently includes pending protocol calls. In the future, more info might be added.
    /// </summary>
    public class DebugInfo
    {
        /// <summary>
        /// Gets the list of pending protocol errors.
        /// </summary>
        public IReadOnlyList<string> PendingProtocolErrors { get; init; } = Array.Empty<string>();
    }
}
