using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.PageCoverage;

namespace PuppeteerSharp.Tests.CSSCoverageTests
{
    public class CSSResetOnNavigationTests : PuppeteerPageBaseTest
    {
        public CSSResetOnNavigationTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs resetOnNavigation", "should report stylesheets across navigations")]
        public async Task ShouldReportStylesheetsAcrossNavigationsWhenDisabled()
        {
            await Page.Coverage.StartCSSCoverageAsync(new CoverageStartOptions
            {
                ResetOnNavigation = false
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/csscoverage/multiple.html");
            await Page.GoToAsync(TestConstants.EmptyPage);
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.AreEqual(2, coverage.Length);
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs resetOnNavigation", "should NOT report scripts across navigations")]
        public async Task ShouldNotReportScriptsAcrossNavigationsWhenEnabled()
        {
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/csscoverage/multiple.html");
            await Page.GoToAsync(TestConstants.EmptyPage);
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.IsEmpty(coverage);
        }
    }
}
