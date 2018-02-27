using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Puppeteer
{
    public class PuppeteerLaunchTests
    {
        public PuppeteerLaunchTests()
        {
            Downloader.CreateDefault().DownloadRevisionAsync(TestConstants.ChromiumRevision).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task ShouldSupportIgnoreHTTPSErrorsOption()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.IgnoreHTTPSErrors = true;

            var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision);
            var page = await browser.NewPageAsync();

            var response = await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");
            Assert.Equal(response.Status.ToString(), "OK");

            await browser.CloseAsync();
        }
    }
}
