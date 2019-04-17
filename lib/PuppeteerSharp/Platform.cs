namespace PuppeteerSharp
{
    /// <summary>
    /// Platform used by a <see cref="BrowserFetcher"/>.
    /// </summary>
    [System.Obsolete("Use PuppeteerSharp.Abstractions.Platform enum instead")]
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
