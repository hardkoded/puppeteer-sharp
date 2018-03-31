using System;
using System.Net;
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

        [Theory]
        [InlineData("domcontentloaded")]
        [InlineData("networkidle0")]
        [InlineData("networkidle2")]
        public async Task ShouldNavigateToEmptyPage(string waitUntil)
        {
            using (var page = await Browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.EmptyPage, new NavigationOptions { WaitUntil =  new[] { waitUntil } });
                Assert.Equal(HttpStatusCode.OK, response.Status);
            }
        }

        [Fact]
        public async Task ShouldFailWhenNavigatingToBadUrl()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await page.GoToAsync("asdfasdf"));
                Assert.Contains("Cannot navigate to invalid URL", exception.Message);
            }
        }
    }
}
