using System.Threading.Tasks;

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
