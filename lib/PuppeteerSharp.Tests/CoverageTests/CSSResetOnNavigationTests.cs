using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CefSharp.DevTools.Dom.PageCoverage;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.CSSCoverageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class CSSResetOnNavigationTests : DevToolsContextBaseTest
    {
        public CSSResetOnNavigationTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("coverage.spec.ts", "resetOnNavigation", "should report stylesheets across navigations")]
        [PuppeteerFact]
        public async Task ShouldReportStylesheetsAcrossNavigationsWhenDisabled()
        {
            await DevToolsContext.Coverage.StartCSSCoverageAsync(new CoverageStartOptions
            {
                ResetOnNavigation = false
            });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/csscoverage/multiple.html");
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var coverage = await DevToolsContext.Coverage.StopCSSCoverageAsync();
            Assert.Equal(2, coverage.Length);
        }

        [PuppeteerTest("coverage.spec.ts", "resetOnNavigation", "should NOT report scripts across navigations")]
        [PuppeteerFact]
        public async Task ShouldNotReportScriptsAcrossNavigationsWhenEnabled()
        {
            await DevToolsContext.Coverage.StartCSSCoverageAsync();
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/csscoverage/multiple.html");
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var coverage = await DevToolsContext.Coverage.StopCSSCoverageAsync();
            Assert.Empty(coverage);
        }
    }
}
