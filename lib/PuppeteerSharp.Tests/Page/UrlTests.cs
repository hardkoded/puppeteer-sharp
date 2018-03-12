using System;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class UrlTests : PuppeteerBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            using (var page = await Browser.NewPageAsync())
            {
                Assert.Equal(TestConstants.AboutBlank, page.Url);
                await page.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal(TestConstants.EmptyPage, page.Url);
            }
        }
    }
}
