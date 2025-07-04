using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.PageCoverage;

namespace PuppeteerSharp.Tests.CoverageTests
{
    public class JSResetOnNavigationTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("coverage.spec", "Coverage specs resetOnNavigation", "should report scripts across navigations when disabled")]
        public async Task ShouldReportScriptsAcrossNavigationsWhenDisabled()
        {
            await Page.Coverage.StartJSCoverageAsync(new CoverageStartOptions
            {
                ResetOnNavigation = false
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/multiple.html");
            await Page.GoToAsync(TestConstants.EmptyPage);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.That(coverage, Has.Length.EqualTo(2));
        }

        [Test, PuppeteerTest("coverage.spec", "Coverage specs resetOnNavigation", "should NOT report scripts across navigations when enabled")]
        public async Task ShouldNotReportScriptsAcrossNavigationsWhenEnabled()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/multiple.html");
            await Page.GoToAsync(TestConstants.EmptyPage);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.That(coverage, Is.Empty);
        }
    }
}
