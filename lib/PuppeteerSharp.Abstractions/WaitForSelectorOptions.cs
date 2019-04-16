namespace PuppeteerSharp.Abstractions
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="IPage.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
    /// <seealso cref="IFrame.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
    public class WaitForSelectorOptions
    {
        /// <summary>
        /// Maximum time to wait for in milliseconds. Defaults to 30000 (30 seconds). Pass 0 to disable timeout.
        /// </summary>
        public int Timeout { get; set; } = Constants.DefaultTimeout;

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