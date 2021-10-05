using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CefSharp.Puppeteer.PageCoverage;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.CoverageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class JSResetOnNavigationTests : PuppeteerPageBaseTest
    {
        public JSResetOnNavigationTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("coverage.spec.ts", "resetOnNavigation", "should report scripts across navigations when disabled")]
        [PuppeteerFact]
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

        [PuppeteerTest("coverage.spec.ts", "resetOnNavigation", "should NOT report scripts across navigations when enabled")]
        [PuppeteerFact]
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
