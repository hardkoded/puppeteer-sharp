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
    }
}