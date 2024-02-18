using System.Threading.Tasks;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.Issues
{
    public class Issue0100 : PuppeteerBrowserContextBaseTest
    {
        public Issue0100() : base()
        {
        }

        public async Task PdfDarkskyShouldWork()
        {
            await using (var page = await Context.NewPageAsync())
            {
                await page.GoToAsync("https://darksky.net/forecast/51.2211,4.3997/si12/en");
                var pdf = await page.PdfDataAsync();
                Assert.NotNull(pdf);
            }
        }
    }
}
