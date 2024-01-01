using System.Threading.Tasks;
using PuppeteerSharp.PageCoverage;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.CSSCoverageTests
{
    public class CSSResetOnNavigationTests : PuppeteerPageBaseTest
    {
        public CSSResetOnNavigationTests(): base()
        {
        }

        [PuppeteerTest("coverage.spec.ts", "resetOnNavigation", "should report stylesheets across navigations")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

        [PuppeteerTest("coverage.spec.ts", "resetOnNavigation", "should NOT report scripts across navigations")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
