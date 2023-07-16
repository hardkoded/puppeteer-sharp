using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Platform used by a <see cref="BrowserFetcher"/>.
    /// </summary>
    public enum Platform
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// MacOS intel.
        /// </summary>
        MacOS,

        /// <summary>
        /// MacOS arm64.
        /// </summary>
        MacOSArm64,

        /// <summary>
        /// Linux.
        /// </summary>
        Linux,

        /// <summary>
        /// Win32.
        /// </summary>
        Win32,

        /// <summary>
        /// Win64.
        /// </summary>
        Win64,
    }
}
