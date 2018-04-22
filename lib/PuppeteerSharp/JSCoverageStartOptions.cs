namespace PuppeteerSharp
{
    /// <summary>
    /// Set of configurable options for coverage
    /// </summary>
    public class JSCoverageStartOptions
    {
        /// <summary>
        /// Whether to reset coverage on every navigation. Defaults to <c>true</c>.
        /// </summary>
        public bool ResetOnNavigation { get; set; } = true;
    }
}