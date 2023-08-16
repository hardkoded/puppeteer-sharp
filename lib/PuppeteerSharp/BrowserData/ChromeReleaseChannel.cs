using System;

namespace PuppeteerSharp.BrowserData
{
    /// <summary>
    /// Chrome release channels.
    /// </summary>
    public enum ChromeReleaseChannel
    {
        /// <summary>
        /// Stable.
        /// </summary>
        Stable = 0,

        /// <summary>
        /// Dev.
        /// </summary>
        Dev = 1,

        /// <summary>
        /// Canary.
        /// </summary>
        Canary = 2,

        /// <summary>
        /// Beta.
        /// </summary>
        Beta = 3,
    }
}
