using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class IsIntersectingViewportTests : PuppeteerPageBaseTest
    {
        public IsIntersectingViewportTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/offscreenbuttons.html");
            for (let i = 0; i < 11; ++i)
            {
                var button = await Page.QuerySelectorAsync('#btn' + i);
                // All but last button are visible.
                var visible = i < 10;
                Assert.True(await button.IsIntersectingViewportAsync());
            }
        }
    }
}