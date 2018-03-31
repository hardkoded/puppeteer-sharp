using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class GotoTests : PuppeteerBaseTest
    {
        [Fact]
        public async Task ShouldNavigateToAboutBlank()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.AboutBlank);
                Assert.Null(response);
            }
        }
    }
}
