using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class ResizeTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("page.spec", "Page Page.resize", "should resize the browser window to fit page content")]
        public async Task ShouldResizeTheBrowserWindowToFitPageContent()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.DefaultViewport = null;

            await using var browser = await Puppeteer.LaunchAsync(options);
            var page = await browser.NewPageAsync();

            var contentWidth = 500;
            var contentHeight = 400;
            var resizedTask = page.EvaluateFunctionAsync(
                "() => new Promise(resolve => { window.onresize = resolve; })");
            await page.ResizeAsync(contentWidth, contentHeight);
            await resizedTask;

            var innerSize = await page.EvaluateFunctionAsync<Size>(
                "() => ({ width: window.innerWidth, height: window.innerHeight })");
            Assert.That(innerSize.Width, Is.EqualTo(contentWidth));
            Assert.That(innerSize.Height, Is.EqualTo(contentHeight));
        }

        private sealed class Size
        {
            public int Width { get; set; }

            public int Height { get; set; }
        }
    }
}
