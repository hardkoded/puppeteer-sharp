using System.Threading.Tasks;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.Issues
{
    public class Issue2251 : PuppeteerPageBaseTest
    {
        public Issue2251() : base()
        {
        }

        public async Task ShouldEvaluateXPathsCorrectly()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            // Test case
            Assert.That(await Page.QuerySelectorAllAsync("xpath///one-app-nav-bar-item-root/button[count(.//*[contains(@icon-name, 'close')]) > 0]"), Is.Empty);
            // check that the Xpath really works
            Assert.That(await Page.QuerySelectorAllAsync("xpath///img"), Is.Not.Empty);
        }
    }
}
