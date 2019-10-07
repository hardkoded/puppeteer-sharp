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
        /// Maximum time to wait for in milliseconds. Pass 0 to disable timeout.
        /// </summary>
        public int? Timeout { get; set; }

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