using System.Threading.Tasks;

namespace PuppeteerSharp.PageCoverage
{
    /// <summary>
    /// gathers information about parts of JavaScript and CSS that were used by the page.
    /// </summary>
    public interface ICoverage
    {
        /// <summary>
        /// Starts CSS coverage.
        /// </summary>
        /// <param name="options">Set of configurable options for coverage.</param>
        /// <returns>A task that resolves when coverage is started.</returns>
        Task StartCSSCoverageAsync(CoverageStartOptions options = null);

        /// <summary>
        /// Starts JS coverage.
        /// </summary>
        /// <param name="options">Set of configurable options for coverage.</param>
        /// <returns>A task that resolves when coverage is started.</returns>
        Task StartJSCoverageAsync(CoverageStartOptions options = null);

        /// <summary>
        /// Stops JS coverage and returns coverage reports for all non-anonymous scripts.
        /// </summary>
        /// <returns>Task that resolves to the array of coverage reports for all stylesheets.</returns>
        /// <remarks>
        /// JavaScript Coverage doesn't include anonymous scripts; however, scripts with sourceURLs are reported.
        /// </remarks>
        Task<CoverageEntry[]> StopCSSCoverageAsync();

        /// <summary>
        /// Stops JS coverage and returns coverage reports for all scripts.
        /// </summary>
        /// <returns>Task that resolves to the array of coverage reports for all stylesheets.</returns>
        /// <remarks>
        /// JavaScript Coverage doesn't include anonymous scripts by default; however, scripts with sourceURLs are reported.
        /// </remarks>
        Task<JSCoverageEntry[]> StopJSCoverageAsync();
    }
}
