namespace PuppeteerSharp.PageCoverage
{
    /// <summary>
    /// Set of configurable options for coverage
    /// </summary>
    public class CoverageStartOptions
    {
        /// <summary>
        /// Whether to reset coverage on every navigation. Defaults to <c>true</c>.
        /// </summary>
        public bool ResetOnNavigation { get; set; } = true;
    }
}