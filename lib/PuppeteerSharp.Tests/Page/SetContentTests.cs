using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class SetContentTests : PuppeteerBaseTest
    {
        const string ExpectedOutput = "<html><head></head><body><div>hello</div></body></html>";

        [Fact]
        public async Task ShouldWork()
        {
            var page = await Browser.NewPageAsync();

            await page.SetContentAsync("<div>hello</div>");
            var result = await page.GetContentAsync();

            Assert.Equal(ExpectedOutput, result);
        }

        [Fact]
        public async Task ShouldWorkWithDoctype()
        {
            var page = await Browser.NewPageAsync();
            const string doctype = "<!DOCTYPE html>";

            await page.SetContentAsync($"{doctype}<div>hello</div>");
            var result = await page.GetContentAsync();

            Assert.Equal($"{doctype}{ExpectedOutput}", result);
        }

        [Fact]
        public async Task ShouldWorkWithHtml4Doctype()
        {
            var page = await Browser.NewPageAsync();
            const string doctype = "<!DOCTYPE html PUBLIC \" -//W3C//DTD HTML 4.01//EN\" " +
                "\"http://www.w3.org/TR/html4/strict.dtd\">";

            await page.SetContentAsync($"{doctype}<div>hello</div>");
            var result = await page.GetContentAsync();

            Assert.Equal($"{doctype}{ExpectedOutput}", result);
        }
    }
}
