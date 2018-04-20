using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// gathers information about parts of JavaScript and CSS that were used by the page.
    /// </summary>
    public class Coverage
    {
        private readonly JSCoverage _jsCoverage;

        internal Coverage(Session client)
        {
            _jsCoverage = new JSCoverage(client);
        }

        /// <summary>
        /// Starts JS coverage
        /// </summary>
        /// <param name="options">Set of configurable options for coverage</param>
        /// <returns>A task that resolves when coverage is started</returns>
        public Task StartJSCoverageAsync(JSCoverageStartOptions options = null)
            => _jsCoverage.StartAsync(options ?? new JSCoverageStartOptions());

        /// <summary>
        /// Stops JS coverage and returns coverage reports for all non-anonymous scripts
        /// </summary>
        /// <returns>Task that resolves to the array of coverage reports for all stylesheets</returns>
        /// <remarks>
        /// JavaScript Coverage doesn't include anonymous scripts; however, scripts with sourceURLs are reported.
        /// </remarks>
        public Task StopJSCoverageAsync() => _jsCoverage.StopAsync();        
    }
}