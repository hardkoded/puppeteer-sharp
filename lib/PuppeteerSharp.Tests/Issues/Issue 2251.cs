using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.Issues
{
    public class Issue2251 : PuppeteerPageBaseTest
    {
        public Issue2251() : base()
        {
        }

        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldEvaluateXPathsCorrectly()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            // Test case
            Assert.IsEmpty(await Page.QuerySelectorAllAsync("xpath///one-app-nav-bar-item-root/button[count(.//*[contains(@icon-name, 'close')]) > 0]"));
            // check that the Xpath really works
            Assert.IsNotEmpty(await Page.QuerySelectorAllAsync("xpath///img"));
        }
    }
}
