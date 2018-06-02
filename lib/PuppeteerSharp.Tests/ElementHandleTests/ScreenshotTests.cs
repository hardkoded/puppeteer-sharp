using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ScreenshotTests : PuppeteerPageBaseTest
    {
        public ScreenshotTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldFire()
        {
            await Page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await Page.EvaluateExpressionAsync("window.scrollBy(50, 100)");
            var elementHandle = await Page.QuerySelectorAsync(".box:nth-of-type(3)");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-element-bounding-box.png", screenshot));
        }

        [Fact]
        public async Task ShouldTakeIntoAccountPaddingAndBorder()
        {
            await Page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await Page.SetContentAsync(@"
                something above
                <style> div {
                    border: 2px solid blue;
                    background: green;
                    width: 50px;
                    height: 50px;
                }
                </style>
                <div></div>
            ");
            var elementHandle = await Page.QuerySelectorAsync("div");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-element-padding-border.png", screenshot));
        }

        [Fact]
        public async Task ShouldScrollElementIntoView()
        {
            await Page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await Page.SetContentAsync(@"
                something above
                <style> div.above {
                    border: 2px solid blue;
                    background: red;
                    height: 1500px;
                }
                div.to-screenshot {
                    border: 2px solid blue;
                    background: green;
                    width: 50px;
                    height: 50px;
                }
                </style>
                <div class='above'></div>
                <div class='to-screenshot'></div>
            ");
            var elementHandle = await Page.QuerySelectorAsync("div.to-screenshot");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-element-scrolled-into-view.png", screenshot));
        }

        [Fact]
        public async Task ShouldWorkWithARotatedElement()
        {
            await Page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await Page.SetContentAsync(@"
                <div style='position: absolute;
                top: 100px;
                left: 100px;
                width: 100px;
                height: 100px;
                background: green;
                transform: rotateZ(200deg); '>&nbsp;</div>
            ");
            var elementHandle = await Page.QuerySelectorAsync("div");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-element-rotate.png", screenshot));
        }

        [Fact]
        public async Task ShouldFailToScreenshotADetachedElement()
        {
            await Page.SetContentAsync("<h1>remove this</h1>");
            var elementHandle = await Page.QuerySelectorAsync("h1");
            await Page.EvaluateFunctionAsync("element => element.remove()", elementHandle);

            var exception = await Assert.ThrowsAsync<PuppeteerException>(elementHandle.ScreenshotStreamAsync);
            Assert.Equal("Node is detached from document", exception.Message);
        }
    }
}
