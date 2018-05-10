using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class BoundingBoxTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            await Page.SetViewport(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var elementHandle = await Page.QuerySelectorAsync(".box:nth-of-type(13)");
            var box = await elementHandle.BoundingBoxAsync();
            Assert.Equal(new BoundingBox(100, 50, 50, 50), box);
        }

        [Fact]
        public async Task ShouldHandleNestedFrames()
        {
            await Page.SetViewport(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            var nestedFrame = Page.Frames[1].ChildFrames[1];
            var elementHandle = await nestedFrame.QuerySelectorAsync("div");
            var box = await elementHandle.BoundingBoxAsync();
            Assert.Equal(new BoundingBox(28, 260, 264, 18), box);
        }

        [Fact]
        public async Task ShouldReturnNullForInvisibleElements()
        {
            await Page.SetContentAsync("<div style='display:none'>hi</div>");
            var elementHandle = await Page.QuerySelectorAsync("div");
            Assert.Null(await elementHandle.BoundingBoxAsync());
        }
    }
}
