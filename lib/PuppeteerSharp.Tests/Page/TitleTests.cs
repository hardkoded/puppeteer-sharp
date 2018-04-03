using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class TitleTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldReturnThePageTitle()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            Assert.Equal("Button test", await Page.GetTitleAsync());
        }
    }
}
