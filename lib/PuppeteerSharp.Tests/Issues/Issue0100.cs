using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Issues
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class Issue0100 : PuppeteerBaseTest
    {
        [Fact(Skip = "It's locking our tests, I'll try to run it again when get have GoToAsync read")]
        public async Task PdfWundergroundShouldWork()
        {
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
            using (var page = await Browser.NewPageAsync())
            {
                await page.GoToAsync("https://darksky.net/forecast/51.2211,4.3997/si12/en");
                var pdf = await page.PdfStreamAsync();
                Assert.NotNull(pdf);
            }
        }
    }
}
