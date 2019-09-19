using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PuppeteerSharp.PageCoverage;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSCoverageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class JSResetOnNavigationTests : PuppeteerPageBaseTest
    {
        public JSResetOnNavigationTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldReportScriptsAcrossNavigationsWhenDisabled()
        {
            await Page.Coverage.StartJSCoverageAsync(new CoverageStartOptions
            {
                ResetOnNavigation = false
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/multiple.html");
            await Page.GoToAsync(TestConstants.EmptyPage);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.Equal(2, coverage.Length);
        }

        [Fact]
        public async Task ShouldNotReportScriptsAcrossNavigationsWhenEnabled()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/multiple.html");
            await Page.GoToAsync(TestConstants.EmptyPage);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.Empty(coverage);
        }
    }
}
