using System;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.HeadfulTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageBringToFrontTests : PuppeteerBaseTest
    {
        public PageBringToFrontTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("headful.spec.ts", "Page.bringToFront", "should work")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            await using (var browserWithExtension = await Puppeteer.LaunchAsync(
                TestConstants.BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory))
            await using (var page = await browserWithExtension.NewPageAsync())
            {
                await page.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal("visible", await page.EvaluateExpressionAsync<string>("document.visibilityState"));

                var newPage = await browserWithExtension.NewPageAsync();
                await newPage.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal("hidden", await page.EvaluateExpressionAsync<string>("document.visibilityState"));
                Assert.Equal("visible", await newPage.EvaluateExpressionAsync<string>("document.visibilityState"));

                await page.BringToFrontAsync();
                Assert.Equal("visible", await page.EvaluateExpressionAsync<string>("document.visibilityState"));
                Assert.Equal("hidden", await newPage.EvaluateExpressionAsync<string>("document.visibilityState"));

                await newPage.CloseAsync();
            }
        }
    }
}
