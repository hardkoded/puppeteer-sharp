namespace PuppeteerSharp
{
    /// <summary>
    /// Information about the browser tab of a page. See <see cref="IPage.TabInfoAsync"/>.
    /// </summary>
    /// <remarks>
    /// This is an experimental API.
    /// </remarks>
    public class TabInfo
    {
        /// <summary>
        /// Gets or sets the zero-based index of the tab in its window's tab strip.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tab is the active tab in its window.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tab is pinned.
        /// </summary>
        public bool Pinned { get; set; }

        /// <summary>
        /// Gets or sets the ID of the tab group the tab belongs to, if any.
        /// </summary>
        public string GroupId { get; set; }
    }
}
