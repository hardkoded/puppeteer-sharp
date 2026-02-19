namespace PuppeteerSharp
{
    /// <summary>
    /// Options for <see cref="IPage.ReloadAsync(ReloadOptions)"/>.
    /// </summary>
    public record ReloadOptions : NavigationOptions
    {
        /// <summary>
        /// If set to true, the browser caches are ignored for the page reload.
        /// </summary>
        /// <value>Defaults to <c>false</c>.</value>
        public bool IgnoreCache { get; set; }
    }
}
