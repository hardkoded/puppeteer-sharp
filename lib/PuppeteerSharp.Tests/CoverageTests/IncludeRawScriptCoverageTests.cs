using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.PageCoverage;

namespace PuppeteerSharp.Tests.CoverageTests
{
    public class IncludeRawScriptCoverageTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs JSCoverage includeRawScriptCoverage", "should not include rawScriptCoverage field when disabled")]
        public async Task ShouldNotIncludeRawScriptCoverageFieldWhenDisabled()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/simple.html", WaitUntilNavigation.Networkidle0);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            Assert.IsNull(coverage[0].RawScriptCoverage);
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs JSCoverage includeRawScriptCoverage", "should include rawScriptCoverage field when enabled")]
        public async Task ShouldIncludeRawScriptCoverageFieldWhenEnabled()
        {
            await Page.Coverage.StartJSCoverageAsync(new CoverageStartOptions
            {
                IncludeRawScriptCoverage = true
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/simple.html", WaitUntilNavigation.Networkidle0);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            Assert.IsNotNull(coverage[0].RawScriptCoverage);
        }
    }
}
