namespace PuppeteerSharp
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="Page.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
    /// <seealso cref="Frame.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
    public class WaitForSelectorOptions
    {
        /// <summary>
        /// Maximum time to wait for in milliseconds. Defaults to 30000 (30 seconds). Pass 0 to disable timeout.
        /// </summary>
        public int Timeout { get; set; } = Puppeteer.DefaultTimeout;

        /// <summary>
        /// Wait for selector to become visible.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Wait for selector to become hidden.
        /// </summary>
        public bool Hidden { get; set; }
    }
}