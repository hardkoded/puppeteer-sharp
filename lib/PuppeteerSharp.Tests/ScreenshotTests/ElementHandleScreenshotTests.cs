using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;
using CefSharp;

namespace PuppeteerSharp.Tests.ScreenshotTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ElementHandleScreenshotTests : DevToolsContextBaseTest
    {
        public ElementHandleScreenshotTests(ITestOutputHelper output) : base(output)
        {
        }

#pragma warning disable IDE0051 // Remove unused private members
        async Task Usage(IWebBrowser chromiumWebBrowser)
#pragma warning restore IDE0051 // Remove unused private members
        {
            #region Screenshot
            //Wait for Initial page load
            await chromiumWebBrowser.WaitForInitialLoadAsync();

            await using var devToolsContext = await chromiumWebBrowser.CreateDevToolsContextAsync();

            await devToolsContext.ScreenshotAsync("file.png");
            #endregion
        }

        [PuppeteerTest("screenshot.spec.ts", "ElementHandle.screenshot", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            #region SetViewportAsync
            // Set Viewport
            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            #endregion
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await DevToolsContext.EvaluateExpressionAsync("window.scrollBy(50, 100)");
            var elementHandle = await DevToolsContext.QuerySelectorAsync(".box:nth-of-type(3)");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-element-bounding-box.png", screenshot));
        }

        [PuppeteerTest("screenshot.spec.ts", "ElementHandle.screenshot", "should take into account padding and border")]
        [PuppeteerFact]
        public async Task ShouldTakeIntoAccountPaddingAndBorder()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await DevToolsContext.SetContentAsync(@"
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
            var elementHandle = await DevToolsContext.QuerySelectorAsync("div");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-element-padding-border.png", screenshot));
        }

        [PuppeteerTest("screenshot.spec.ts", "ElementHandle.screenshot", "should capture full element when larger than viewport")]
        [PuppeteerFact(Skip = "TODO: CEF")]
        public async Task ShouldCaptureFullElementWhenLargerThanViewport()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await DevToolsContext.SetContentAsync(@"
                something above
                <style>
                div.to-screenshot {
                  border: 1px solid blue;
                  width: 600px;
                  height: 600px;
                  margin-left: 50px;
                }
                ::-webkit-scrollbar{
                  display: none;
                }
                </style>
                <div class='to-screenshot'></div>"
            );
            var elementHandle = await DevToolsContext.QuerySelectorAsync("div.to-screenshot");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-element-larger-than-viewport.png", screenshot));
            Assert.Equal(JToken.FromObject(new { w = 500, h = 500 }),
                await DevToolsContext.EvaluateExpressionAsync("({ w: window.innerWidth, h: window.innerHeight })"));
        }

        [PuppeteerFact]
        public async Task ShouldScrollElementIntoView()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await DevToolsContext.SetContentAsync(@"
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
            var elementHandle = await DevToolsContext.QuerySelectorAsync("div.to-screenshot");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-element-scrolled-into-view.png", screenshot));
        }

        [PuppeteerFact]
        public async Task ShouldWorkWithARotatedElement()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await DevToolsContext.SetContentAsync(@"
                <div style='position: absolute;
                top: 100px;
                left: 100px;
                width: 100px;
                height: 100px;
                background: green;
                transform: rotateZ(200deg); '>&nbsp;</div>
            ");
            var elementHandle = await DevToolsContext.QuerySelectorAsync("div");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-element-rotate.png", screenshot));
        }

        [PuppeteerFact]
        public async Task ShouldFailToScreenshotADetachedElement()
        {
            await DevToolsContext.SetContentAsync("<h1>remove this</h1>");
            var elementHandle = await DevToolsContext.QuerySelectorAsync("h1");
            await DevToolsContext.EvaluateFunctionAsync("element => element.remove()", elementHandle);

            var exception = await Assert.ThrowsAsync<PuppeteerException>(elementHandle.ScreenshotStreamAsync);
            Assert.Equal("Node is either not visible or not an HTMLElement", exception.Message);
        }

        [PuppeteerFact]
        public async Task ShouldNotHangWithZeroWidthHeightElement()
        {
            await DevToolsContext.SetContentAsync(@"<div style='width: 50px; height: 0'></div>");
            var elementHandle = await DevToolsContext.QuerySelectorAsync("div");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(elementHandle.ScreenshotDataAsync);
            Assert.Equal("Node has 0 height.", exception.Message);
        }

        [PuppeteerFact]
        public async Task ShouldWorkForAnElementWithFractionalDimensions()
        {
            await DevToolsContext.SetContentAsync("<div style=\"width:48.51px;height:19.8px;border:1px solid black;\"></div>");
            var elementHandle = await DevToolsContext.QuerySelectorAsync("div");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-element-fractional.png", screenshot));
        }

        [PuppeteerFact]
        public async Task ShouldWorkForAnElementWithAnOffset()
        {
            await DevToolsContext.SetContentAsync("<div style=\"position:absolute; top: 10.3px; left: 20.4px;width:50.3px;height:20.2px;border:1px solid black;\"></div>");
            var elementHandle = await DevToolsContext.QuerySelectorAsync("div");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-element-fractional-offset.png", screenshot));
        }
    }
}
