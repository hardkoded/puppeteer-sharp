using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class ClickablePointTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.clickablePoint", "should work")]
        public async Task ShouldWork()
        {
            await Page.EvaluateExpressionAsync(@"
                document.body.style.padding = '0';
                document.body.style.margin = '0';
                document.body.innerHTML = '<div style=""cursor: pointer; width: 120px; height: 60px; margin: 30px; padding: 15px;""></div>';
            ");

            await Page.EvaluateExpressionAsync(
                "new Promise(resolve => window.requestAnimationFrame(resolve))");

            var divHandle = await Page.QuerySelectorAsync("div");

            var point = await divHandle.ClickablePointAsync();
            Assert.That(point.X, Is.EqualTo(45 + 60)); // margin + middle point offset
            Assert.That(point.Y, Is.EqualTo(45 + 30)); // margin + middle point offset

            var pointWithOffset = await divHandle.ClickablePointAsync(new Offset(10, 15));
            Assert.That(pointWithOffset.X, Is.EqualTo(30 + 10)); // margin + offset
            Assert.That(pointWithOffset.Y, Is.EqualTo(30 + 15)); // margin + offset
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.clickablePoint", "should work for iframes")]
        public async Task ShouldWorkForIframes()
        {
            await Page.EvaluateExpressionAsync(@"
                document.body.style.padding = '10px';
                document.body.style.margin = '10px';
                document.body.innerHTML = '<iframe style=""border: none; margin: 0; padding: 0;"" seamless sandbox srcdoc=""<style>* { margin: 0; padding: 0;}</style><div style=\'cursor: pointer; width: 120px; height: 60px; margin: 30px; padding: 15px;\' />""></iframe>';
            ");

            await Page.EvaluateExpressionAsync(
                "new Promise(resolve => window.requestAnimationFrame(resolve))");

            var frame = Page.Frames[1];
            var divHandle = await frame.QuerySelectorAsync("div");

            var point = await divHandle.ClickablePointAsync();
            Assert.That(point.X, Is.EqualTo(20 + 45 + 60)); // iframe pos + margin + middle point offset
            Assert.That(point.Y, Is.EqualTo(20 + 45 + 30)); // iframe pos + margin + middle point offset

            var pointWithOffset = await divHandle.ClickablePointAsync(new Offset(10, 15));
            Assert.That(pointWithOffset.X, Is.EqualTo(20 + 30 + 10)); // iframe pos + margin + offset
            Assert.That(pointWithOffset.Y, Is.EqualTo(20 + 30 + 15)); // iframe pos + margin + offset
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.clickablePoint", "should not work if the click box is not visible")]
        public async Task ShouldNotWorkIfTheClickBoxIsNotVisible()
        {
            await Page.SetContentAsync(
                "<button style=\"width: 10px; height: 10px; position: absolute; left: -20px\"></button>");
            var handle = await Page.WaitForSelectorAsync("button");
            Assert.ThrowsAsync<PuppeteerException>(async () => await handle.ClickablePointAsync());

            await Page.SetContentAsync(
                "<button style=\"width: 10px; height: 10px; position: absolute; right: -20px\"></button>");
            var handle2 = await Page.WaitForSelectorAsync("button");
            Assert.ThrowsAsync<PuppeteerException>(async () => await handle2.ClickablePointAsync());

            await Page.SetContentAsync(
                "<button style=\"width: 10px; height: 10px; position: absolute; top: -20px\"></button>");
            var handle3 = await Page.WaitForSelectorAsync("button");
            Assert.ThrowsAsync<PuppeteerException>(async () => await handle3.ClickablePointAsync());

            await Page.SetContentAsync(
                "<button style=\"width: 10px; height: 10px; position: absolute; bottom: -20px\"></button>");
            var handle4 = await Page.WaitForSelectorAsync("button");
            Assert.ThrowsAsync<PuppeteerException>(async () => await handle4.ClickablePointAsync());
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.clickablePoint", "should not work if the click box is not visible due to the iframe")]
        [System.Obsolete]
        public async Task ShouldNotWorkIfTheClickBoxIsNotVisibleDueToTheIframe()
        {
            await Page.SetContentAsync(
                "<iframe name=\"frame\" style=\"position: absolute; left: -100px\" srcdoc=\"<button style='width: 10px; height: 10px;'></button>\"></iframe>");
            var frame = await Page.WaitForFrameAsync(f => f.Name == "frame");
            var handle = await frame.WaitForSelectorAsync("button");
            Assert.ThrowsAsync<PuppeteerException>(async () => await handle.ClickablePointAsync());

            await Page.SetContentAsync(
                "<iframe name=\"frame2\" style=\"position: absolute; top: -100px\" srcdoc=\"<button style='width: 10px; height: 10px;'></button>\"></iframe>");
            var frame2 = await Page.WaitForFrameAsync(f => f.Name == "frame2");
            var handle2 = await frame2.WaitForSelectorAsync("button");
            Assert.ThrowsAsync<PuppeteerException>(async () => await handle2.ClickablePointAsync());
        }
    }
}
