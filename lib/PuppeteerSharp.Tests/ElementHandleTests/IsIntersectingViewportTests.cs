using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class IsIntersectingViewportTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.isIntersectingViewport", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/offscreenbuttons.html");
            for (var i = 0; i < 11; ++i)
            {
                var button = await Page.QuerySelectorAsync("#btn" + i);
                // All but last button are visible.
                var visible = i < 10;
                Assert.AreEqual(visible, await button.IsIntersectingViewportAsync());
            }
        }

        [Test, Retry(2), PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.isIntersectingViewport", "should work with threshold")]
        public async Task ShouldWorkWithThreshold()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/offscreenbuttons.html");
            var button = await Page.QuerySelectorAsync("#btn11");
            Assert.False(await button.IsIntersectingViewportAsync(0.001m));
        }

        [Test, Retry(2), PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.isIntersectingViewport", "should work with threshold of 1")]
        public async Task ShouldWorkWithThresholdOf1()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/offscreenbuttons.html");
            var button = await Page.QuerySelectorAsync("#btn0");
            Assert.True(await button.IsIntersectingViewportAsync(1));
        }

        [Test, Retry(2), PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.isIntersectingViewport", "should work with svg elements")]
        public async Task ShouldWorkWithSvgElements()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/inline-svg.html");
            var visibleCircle = await Page.QuerySelectorAsync("circle");
            var visibleSvg = await Page.QuerySelectorAsync("svg");

            Assert.True(await visibleCircle.IsIntersectingViewportAsync(1));
            Assert.True(await visibleSvg.IsIntersectingViewportAsync(1));
            Assert.True(await visibleCircle.IsIntersectingViewportAsync());
            Assert.True(await visibleSvg.IsIntersectingViewportAsync());

            var invisibleCircle = await Page.QuerySelectorAsync("div circle");
            var invisibleSvg = await Page.QuerySelectorAsync("div svg");

            Assert.False(await invisibleCircle.IsIntersectingViewportAsync(1));
            Assert.False(await invisibleSvg.IsIntersectingViewportAsync(1));
            Assert.False(await invisibleCircle.IsIntersectingViewportAsync());
            Assert.False(await invisibleSvg.IsIntersectingViewportAsync());
        }
    }
}
