namespace PuppeteerSharp
{
    /// <summary>
    /// Browser options.
    /// </summary>
    public interface IBrowserOptions
    {
        /// <summary>
        /// Whether to ignore HTTPS errors during navigation. Defaults to false.
        /// </summary>
        bool IgnoreHTTPSErrors { get; }

        /// <summary>
        /// If set to true, sets Headless = false, otherwise, enables automation.
        /// </summary>
        bool AppMode { get; }
    }
}
