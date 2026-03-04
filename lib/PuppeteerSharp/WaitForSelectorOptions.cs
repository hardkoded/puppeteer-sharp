namespace PuppeteerSharp
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="IPage.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
    /// <seealso cref="IFrame.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
    public class WaitForSelectorOptions : WaitForOptions
    {
        /// <summary>
        /// Wait for element to be present in DOM and to be visible.
        /// </summary>
        public bool? Visible { get; set; }

        /// <summary>
        /// Wait for element to not be found in the DOM or to be hidden.
        /// </summary>
        public bool? Hidden { get; set; }

        /// <summary>
        /// Root element.
        /// </summary>
        public IElementHandle Root { get; set; }
    }
}
