namespace PuppeteerSharp.PageAccessibility
{
    /// <summary>
    /// <see cref="Accessibility.SnapshotAsync(AccessibilitySnapshotOptions)"/>
    /// </summary>
    /// <seealso cref="Page.Accessibility"/>
    public class AccessibilitySnapshotOptions
    {
        /// <summary>
        /// Prune uninteresting nodes from the tree. Defaults to true.
        /// </summary>
        public bool InterestingOnly { get; set; } = true;
        /// <summary>
        /// The root DOM element for the snapshot. Defaults to the whole page.
        /// </summary>
        public ElementHandle Root { get; set; }
    }
}
