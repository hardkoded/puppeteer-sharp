using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NavigationTests
{
    public class NetworkEnabledTests : PuppeteerBaseTest
    {
        [Test, PuppeteerTest("navigation.spec", "with network events disabled", "should work")]
        public async Task ShouldWork()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.NetworkEnabled = false;
            await using var browser = await Puppeteer.LaunchAsync(options);
            var page = await browser.NewPageAsync();

            var response = await page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(response, Is.Null);
            Assert.That(page.Url, Is.EqualTo(TestConstants.EmptyPage));
            Assert.That(
                await page.EvaluateExpressionAsync<string>("window.location.href"),
                Is.EqualTo(TestConstants.EmptyPage));
        }
    }
}
