using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace PuppeteerSharp.Tests.CSSCoverageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class CSSResetOnNavigationTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldReportStylesheetsAcrossNavigationsWhenDisabled()
        {
            await Page.Coverage.StartCSSCoverageAsync(new JSCoverageStartOptions
            {
                ResetOnNavigation = false
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/csscoverage/multiple.html");
            await Page.GoToAsync(TestConstants.EmptyPage);
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.Equal(2, coverage.Length);
        }

        [Fact]
        public async Task ShouldNotReportScriptsAcrossNavigationsWhenEnabled()
        {
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/csscoverage/multiple.html");
            await Page.GoToAsync(TestConstants.EmptyPage);
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.Empty(coverage);
        }
    }
}
