using System.Threading.Tasks;
using PuppeteerSharp.Abstractions.PageCoverage;

namespace PuppeteerSharp.Abstractions
{
    interface ICoverage
    {
        Task StartJSCoverageAsync(CoverageStartOptions options = null);
        Task<CoverageEntry[]> StopJSCoverageAsync();
        Task StartCSSCoverageAsync(CoverageStartOptions options = null);
        Task<CoverageEntry[]> StopCSSCoverageAsync();
    }

}
