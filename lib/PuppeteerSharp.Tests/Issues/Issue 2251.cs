using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.Issues
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class Issue2251 : PuppeteerPageBaseTest
    {
        public Issue2251(ITestOutputHelper output) : base(output)
        {
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldEvaluateXPathsCorrectly()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            // Test case
            Assert.Empty(await Page.QuerySelectorAllAsync("xpath///one-app-nav-bar-item-root/button[count(.//*[contains(@icon-name, 'close')]) > 0]"));
            // check that the Xpath really works
            Assert.NotEmpty(await Page.QuerySelectorAllAsync("xpath///img"));
        }
    }
}
