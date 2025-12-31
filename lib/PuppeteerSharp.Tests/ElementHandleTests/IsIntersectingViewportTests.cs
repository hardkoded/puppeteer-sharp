using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class IsIntersectingViewportTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.isIntersectingViewport", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/offscreenbuttons.html");
            for (var i = 0; i < 11; ++i)
            {
                var button = await Page.QuerySelectorAsync("#btn" + i);
                // All but last button are visible.
                var visible = i < 10;
                Assert.That(await button.IsIntersectingViewportAsync(), Is.EqualTo(visible));
            }
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.isIntersectingViewport", "should work with threshold")]
        public async Task ShouldWorkWithThreshold()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/offscreenbuttons.html");
            var button = await Page.QuerySelectorAsync("#btn11");
            Assert.That(await button.IsIntersectingViewportAsync(0.001m), Is.False);
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.isIntersectingViewport", "should work with threshold of 1")]
        public async Task ShouldWorkWithThresholdOf1()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/offscreenbuttons.html");
            var button = await Page.QuerySelectorAsync("#btn0");
            Assert.That(await button.IsIntersectingViewportAsync(1), Is.True);
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.isIntersectingViewport", "should work with svg elements")]
        public async Task ShouldWorkWithSvgElements()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/inline-svg.html");
            var visibleCircle = await Page.QuerySelectorAsync("circle");
            var visibleSvg = await Page.QuerySelectorAsync("svg");

            Assert.That(await visibleCircle.IsIntersectingViewportAsync(1), Is.True);
            Assert.That(await visibleSvg.IsIntersectingViewportAsync(1), Is.True);
            Assert.That(await visibleCircle.IsIntersectingViewportAsync(), Is.True);
            Assert.That(await visibleSvg.IsIntersectingViewportAsync(), Is.True);

            var invisibleCircle = await Page.QuerySelectorAsync("div circle");
            var invisibleSvg = await Page.QuerySelectorAsync("div svg");

            Assert.That(await invisibleCircle.IsIntersectingViewportAsync(1), Is.False);
            Assert.That(await invisibleSvg.IsIntersectingViewportAsync(1), Is.False);
            Assert.That(await invisibleCircle.IsIntersectingViewportAsync(), Is.False);
            Assert.That(await invisibleSvg.IsIntersectingViewportAsync(), Is.False);
        }
    }
}
