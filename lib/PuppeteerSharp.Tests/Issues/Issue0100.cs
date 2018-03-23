using System;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Issues
{
    public class Issue0100 : PuppeteerBaseTest
    {
        public Issue0100()
        {
            Downloader.CreateDefault().DownloadRevisionAsync(TestConstants.ChromiumRevision).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task PdfWundergroundShouldWork()
        {
            using (var browser = PuppeteerSharp.Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(),
                                                                     TestConstants.ChromiumRevision))
            using (var page = await Browser.NewPageAsync())
            {
                var response = await page.GoToAsync("https://www.wunderground.com/weather/be/antwerp");
                var pdf = await page.PdfStreamAsync();
                Assert.NotNull(pdf);
            }
        }

        [Fact]
        public async Task PdfDarkskyShouldWork()
        {
            using (var browser = PuppeteerSharp.Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(),
                                                                     TestConstants.ChromiumRevision))
            using (var page = await Browser.NewPageAsync())
            {
                await page.GoToAsync("https://darksky.net/forecast/51.2211,4.3997/si12/en");
                var pdf = await page.PdfStreamAsync();
                Assert.NotNull(pdf);

            }
        }
    }
}
