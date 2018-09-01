using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp.PageCoverage
{
    /// <summary>
    /// gathers information about parts of JavaScript and CSS that were used by the page.
    /// </summary>
    public class Coverage
    {
        private readonly JSCoverage _jsCoverage;
        private readonly CSSCoverage _cssCoverage;

        internal Coverage(CDPSession client)
        {
            _jsCoverage = new JSCoverage(client);
            _cssCoverage = new CSSCoverage(client);
        }

        /// <summary>
        /// Starts JS coverage
        /// </summary>
        /// <param name="options">Set of configurable options for coverage</param>
        /// <returns>A task that resolves when coverage is started</returns>
        public Task StartJSCoverageAsync(CoverageStartOptions options = null)
            => _jsCoverage.StartAsync(options ?? new CoverageStartOptions());

        /// <summary>
        /// Stops JS coverage and returns coverage reports for all scripts
        /// </summary>
        /// <returns>Task that resolves to the array of coverage reports for all stylesheets</returns>
        /// <remarks>
        /// JavaScript Coverage doesn't include anonymous scripts by default; however, scripts with sourceURLs are reported.
        /// </remarks>
        public Task<CoverageEntry[]> StopJSCoverageAsync() => _jsCoverage.StopAsync();

        /// <summary>
        /// Starts CSS coverage
        /// </summary>
        /// <param name="options">Set of configurable options for coverage</param>
        /// <returns>A task that resolves when coverage is started</returns>
        public Task StartCSSCoverageAsync(CoverageStartOptions options = null)
            => _cssCoverage.StartAsync(options ?? new CoverageStartOptions());

        /// <summary>
        /// Stops JS coverage and returns coverage reports for all non-anonymous scripts
        /// </summary>
        /// <returns>Task that resolves to the array of coverage reports for all stylesheets</returns>
        /// <remarks>
        /// JavaScript Coverage doesn't include anonymous scripts; however, scripts with sourceURLs are reported.
        /// </remarks>
        public Task<CoverageEntry[]> StopCSSCoverageAsync() => _cssCoverage.StopAsync();

        internal static CoverageEntryRange[] ConvertToDisjointRanges(List<CoverageResponseRange> nestedRanges)
        {
            var points = new List<CoverageEntryPoint>();
            foreach (var range in nestedRanges)
            {
                points.Add(new CoverageEntryPoint
                {
                    Offset = range.StartOffset,
                    Type = 0,
                    Range = range
                });

                points.Add(new CoverageEntryPoint
                {
                    Offset = range.EndOffset,
                    Type = 1,
                    Range = range
                });
            }

            points.Sort();

            var hitCountStack = new List<int>();
            var results = new List<CoverageEntryRange>();
            var lastOffset = 0;

            // Run scanning line to intersect all ranges.
            foreach (var point in points)
            {
                if (hitCountStack.Count > 0 && lastOffset < point.Offset && hitCountStack[hitCountStack.Count - 1] > 0)
                {
                    var lastResult = results.Count > 0 ? results[results.Count - 1] : null;
                    if (lastResult != null && lastResult.End == lastOffset)
                    {
                        lastResult.End = point.Offset;
                    }
                    else
                    {
                        results.Add(new CoverageEntryRange
                        {
                            Start = lastOffset,
                            End = point.Offset
                        });
                    }
                }

                lastOffset = point.Offset;
                if (point.Type == 0)
                {
                    hitCountStack.Add(point.Range.Count);
                }
                else
                {
                    hitCountStack.RemoveAt(hitCountStack.Count - 1);
                }
            }
            // Filter out empty ranges.
            return results.Where(range => range.End - range.Start > 1).ToArray();
        }
    }
}