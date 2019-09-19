using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
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

        [Fact]
        public async Task ShouldWorkWithHistoryAPI()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateExpressionAsync(@"
              history.pushState({ }, '', '/first.html');
              history.pushState({ }, '', '/second.html');
            ");
            Assert.Equal(TestConstants.ServerUrl + "/second.html", Page.Url);

            await Page.GoBackAsync();
            Assert.Equal(TestConstants.ServerUrl + "/first.html", Page.Url);
            await Page.GoBackAsync();
            Assert.Equal(TestConstants.EmptyPage, Page.Url);
            await Page.GoForwardAsync();
            Assert.Equal(TestConstants.ServerUrl + "/first.html", Page.Url);
        }
    }
}