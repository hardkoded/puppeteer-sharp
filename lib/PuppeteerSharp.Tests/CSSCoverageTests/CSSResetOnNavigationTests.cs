using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PuppeteerSharp.PageCoverage;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.CSSCoverageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class CSSResetOnNavigationTests : PuppeteerPageBaseTest
    {
        public CSSResetOnNavigationTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldReportStylesheetsAcrossNavigationsWhenDisabled()
        {
            await Page.Coverage.StartCSSCoverageAsync(new CoverageStartOptions
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
