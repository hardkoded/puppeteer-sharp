using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ScreenshotTests
{
    public class ElementHandleScreenshotTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots ElementHandle.screenshot", "should work")]
        public async Task ShouldWork()
        {
            #region SetViewportAsync
            await Page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            #endregion
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await Page.EvaluateExpressionAsync("window.scrollBy(50, 100)");
            var elementHandle = await Page.QuerySelectorAsync(".box:nth-of-type(3)");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-element-bounding-box.png", screenshot));
        }

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots ElementHandle.screenshot", "should take into account padding and border")]
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

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots ElementHandle.screenshot", "should capture full element when larger than viewport")]
        public async Task ShouldCaptureFullElementWhenLargerThanViewport()
        {
            await Page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await Page.SetContentAsync(@"
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
            var elementHandle = await Page.QuerySelectorAsync("div.to-screenshot");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-element-larger-than-viewport.png", screenshot));
            Assert.AreEqual(JToken.FromObject(new { w = 500, h = 500 }),
                await Page.EvaluateExpressionAsync("({ w: window.innerWidth, h: window.innerHeight })"));
        }

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots ElementHandle.screenshot", "should scroll element into view")]
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

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots ElementHandle.screenshot", "should work with a rotated element")]
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

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots ElementHandle.screenshot", "should fail to screenshot a detached element")]
        public async Task ShouldFailToScreenshotADetachedElement()
        {
            await Page.SetContentAsync("<h1>remove this</h1>");
            var elementHandle = await Page.QuerySelectorAsync("h1");
            await Page.EvaluateFunctionAsync("element => element.remove()", elementHandle);

            var exception = Assert.ThrowsAsync<PuppeteerException>(elementHandle.ScreenshotStreamAsync);
            Assert.AreEqual("Node is either not visible or not an HTMLElement", exception!.Message);
        }

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots ElementHandle.screenshot", "should not hang with zero width/height element")]
        public async Task ShouldNotHangWithZeroWidthHeightElement()
        {
            await Page.SetContentAsync(@"<div style='width: 50px; height: 0'></div>");
            var elementHandle = await Page.QuerySelectorAsync("div");
            var exception = Assert.ThrowsAsync<PuppeteerException>(elementHandle.ScreenshotDataAsync);
            Assert.AreEqual("Node has 0 height.", exception!.Message);
        }

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots ElementHandle.screenshot", "should work for an element with fractional dimensions")]
        public async Task ShouldWorkForAnElementWithFractionalDimensions()
        {
            await Page.SetContentAsync("<div style=\"width:48.51px;height:19.8px;border:1px solid black;\"></div>");
            var elementHandle = await Page.QuerySelectorAsync("div");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-element-fractional.png", screenshot));
        }

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots ElementHandle.screenshot", "should work for an element with an offset")]
        public async Task ShouldWorkForAnElementWithAnOffset()
        {
            await Page.SetContentAsync("<div style=\"position:absolute; top: 10.3px; left: 20.4px;width:50.3px;height:20.2px;border:1px solid black;\"></div>");
            var elementHandle = await Page.QuerySelectorAsync("div");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-element-fractional-offset.png", screenshot));
        }

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots ElementHandle.screenshot", "should work with a null viewport")]
        public async Task ShouldWorkWithANullViewport()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.DefaultViewport = null;
            await using var browser = await Puppeteer.LaunchAsync(options);
            var page = await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await page.EvaluateExpressionAsync("window.scrollBy(50, 100)");
            var elementHandle = await page.QuerySelectorAsync(".box:nth-of-type(3)");
            var screenshot = await elementHandle.ScreenshotDataAsync();
            Assert.Greater(screenshot.Length, 0);
        }
    }
}
