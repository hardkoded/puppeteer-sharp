using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Browser tag.
    /// </summary>
    public enum BrowserTag
    {
        /// <summary>
        /// Latest version.
        /// </summary>
        Latest = 0,

        /// <summary>
        /// Beta version.
        /// </summary>
        Beta = 1,

        /// <summary>
        /// Canary version.
        /// </summary>
        Canary = 2,

        /// <summary>
        /// Dev version.
        /// </summary>
        Dev = 3,

        /// <summary>
        /// Stable version.
        /// </summary>
        Stable = 4,
    }
}
