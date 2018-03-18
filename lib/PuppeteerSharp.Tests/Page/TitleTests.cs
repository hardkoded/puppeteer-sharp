using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class TitleTests : PuppeteerBaseTest
    {
        [Fact]
        public async Task ShouldReturnThePageTitle()
        {
            var page = await Browser.NewPageAsync();

            await page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            Assert.Equal("Button test", await page.GetTitleAsync());
        }
    }
}
