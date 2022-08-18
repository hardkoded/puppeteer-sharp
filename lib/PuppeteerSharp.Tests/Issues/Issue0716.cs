using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.Issues
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class Issue0716 : PuppeteerPageBaseTest
    {
        public Issue0716(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact(Timeout = -1)]
        public async Task ShouldWorkInSlowMo()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.SlowMo = 100;
            options.Headless = false;

            await using (var browser = await Puppeteer.LaunchAsync(options))
            await using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync("https://duckduckgo.com/");
                var input = await page.WaitForSelectorAsync("input[type=\"text\"");
                await input.TypeAsync("Lorem ipsum dolor sit amet.");
            }
        }
    }
}
