using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class OfflineModeTests : PuppeteerPageBaseTest
    {
        public OfflineModeTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            await Page.SetOfflineModeAsync(true);
            await Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync(TestConstants.EmptyPage));

            await Page.SetOfflineModeAsync(false);
            var response = await Page.ReloadAsync();
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public async Task ShouldEmulateNavigatorOnLine()
        {
            Assert.True(await Page.EvaluateExpressionAsync<bool>("window.navigator.onLine"));

            await Page.SetOfflineModeAsync(true);
            Assert.False(await Page.EvaluateExpressionAsync<bool>("window.navigator.onLine"));

            await Page.SetOfflineModeAsync(false);
            Assert.True(await Page.EvaluateExpressionAsync<bool>("window.navigator.onLine"));
        }
    }
}