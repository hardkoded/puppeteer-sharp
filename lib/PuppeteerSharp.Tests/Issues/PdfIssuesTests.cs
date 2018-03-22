using System;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Issues
{
    public class PdfIssuesTests : PuppeteerBaseTest
    {
        public PdfIssuesTests()
        {
            Downloader.CreateDefault().DownloadRevisionAsync(TestConstants.ChromiumRevision).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task Issue100()
        {
            using (var browser = PuppeteerSharp.Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(),
                                                                     TestConstants.ChromiumRevision))
            using (var page = await Browser.NewPageAsync())
            {
                var response = await page.GoToAsync("https://www.wunderground.com/weather/be/antwerp");
                var pdf = await page.PdfStreamAsync();
                Assert.NotNull(pdf);

                await page.GoToAsync("https://darksky.net/forecast/51.2211,4.3997/si12/en");
                pdf = await page.PdfStreamAsync();
                Assert.NotNull(pdf);

            }
        }
    }
}
