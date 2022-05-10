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
        /// Maximum time to wait for in milliseconds. Defaults to `30000` (30 seconds).
        /// Pass `0` to disable timeout.
        /// The default value can be changed by using <seealso cref="Page.DefaultTimeout"/>  method
        /// </summary>
        public int? Timeout { get; set; }

        /// <summary>
        /// Wait for element to be present in DOM and to be visible.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Wait for element to not be found in the DOM or to be hidden.
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Root element
        /// </summary>
        public ElementHandle Root { get; set; }
    }
}
