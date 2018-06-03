using System;
namespace PuppeteerSharp
{
    /// <summary>
    /// Platform used by a <see cref="Downloader"/>.
    /// </summary>
    public enum Platform
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown,
        /// <summary>
        /// MacOS.
        /// </summary>
        MacOS,
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
        Win64
    }
}
