using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.Issues
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class Issue0716 : PuppeteerPageBaseTest
    {
        public Issue0716(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWorkInSlowMo()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.SlowMo = 100;
            options.Headless = false;

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync("https://duckduckgo.com/");
                var input = await page.WaitForSelectorAsync("#search_form_input_homepage");
                await input.TypeAsync("Lorem ipsum dolor sit amet.");
            }
        }
    }
}