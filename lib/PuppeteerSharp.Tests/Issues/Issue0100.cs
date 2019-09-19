using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.Issues
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class Issue0100 : PuppeteerBrowserContextBaseTest
    {
        public Issue0100(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task PdfDarkskyShouldWork()
        {
            using (var page = await Context.NewPageAsync())
            {
                await page.GoToAsync("https://darksky.net/forecast/51.2211,4.3997/si12/en");
                var pdf = await page.PdfDataAsync();
                Assert.NotNull(pdf);
            }
        }
    }
}
