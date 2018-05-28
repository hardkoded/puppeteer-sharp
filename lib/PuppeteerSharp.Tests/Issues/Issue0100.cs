using System;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Issues
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class Issue0100 : PuppeteerBrowserBaseTest
    {
        [Fact]
        public async Task PdfDarkskyShouldWork()
        {
            using (var page = await Browser.NewPageAsync())
            {
                await page.GoToAsync("https://darksky.net/forecast/51.2211,4.3997/si12/en");
                var pdf = await page.PdfDataAsync();
                Assert.NotNull(pdf);
            }
        }
    }
}
