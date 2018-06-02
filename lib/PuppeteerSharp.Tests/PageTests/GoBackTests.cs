using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class GoBackTests : PuppeteerPageBaseTest
    {
        public GoBackTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");

            var response = await Page.GoBackAsync();
            Assert.True(response.Ok);
            Assert.Equal(TestConstants.EmptyPage, response.Url);

            response = await Page.GoForwardAsync();
            Assert.True(response.Ok);
            Assert.Contains("grid", response.Url);

            response = await Page.GoForwardAsync();
            Assert.Null(response);
        }
    }
}
