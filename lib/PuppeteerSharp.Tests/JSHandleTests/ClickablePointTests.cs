using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class ClickablePointTests : PuppeteerPageBaseTest
    {
        public ClickablePointTests() : base()
        {
        }

        [Test, PuppeteerTest("jshandle.spec", "JSHandle JSHandle.clickablePoint", "should work")]
        public async Task ShouldWork()
        {
            await Page.EvaluateExpressionAsync(@"document.body.style.padding = '0';
                document.body.style.margin = '0';
                document.body.innerHTML = '<div style=""cursor: pointer; width: 120px; height: 60px; margin: 30px; padding: 15px;""></div>';
                ");

            await Page.EvaluateExpressionAsync("new Promise(resolve => requestAnimationFrame(() => requestAnimationFrame(resolve)));");

            var divHandle = await Page.QuerySelectorAsync("div");

            var clickablePoint = await divHandle.ClickablePointAsync();

            // margin + middle point offset
            Assert.That(clickablePoint.X, Is.EqualTo(45 + 60));
            Assert.That(clickablePoint.Y, Is.EqualTo(45 + 30));

            clickablePoint = await divHandle.ClickablePointAsync(new Offset { X = 10, Y = 15 });

            // margin + offset
            Assert.That(clickablePoint.X, Is.EqualTo(30 + 10));
            Assert.That(clickablePoint.Y, Is.EqualTo(30 + 15));
        }

        [Test, PuppeteerTest("jshandle.spec", "JSHandle JSHandle.clickablePoint", "should work for iframes")]
        public async Task ShouldWorkForIFrames()
        {
            await Page.EvaluateExpressionAsync(@"document.body.style.padding = '10px';
                document.body.style.margin = '10px';
                document.body.innerHTML = `<iframe style=""border: none; margin: 0; padding: 0;"" seamless sandbox srcdoc=""<style>* { margin: 0; padding: 0;}</style><div style='cursor: pointer; width: 120px; height: 60px; margin: 30px; padding: 15px;' />""></iframe>`
                ");

            await Page.EvaluateExpressionAsync("new Promise(resolve => requestAnimationFrame(() => requestAnimationFrame(resolve)));");

            var frame = Page.FirstChildFrame();

            var divHandle = await frame.QuerySelectorAsync("div");

            var clickablePoint = await divHandle.ClickablePointAsync();

            // iframe pos + margin + middle point offset
            Assert.That(clickablePoint.X, Is.EqualTo(20 + 45 + 60));
            Assert.That(clickablePoint.Y, Is.EqualTo(20 + 45 + 30));

            clickablePoint = await divHandle.ClickablePointAsync(new Offset { X = 10, Y = 15 });

            // iframe pos + margin + offset
            Assert.That(clickablePoint.X, Is.EqualTo(20 + 30 + 10));
            Assert.That(clickablePoint.Y, Is.EqualTo(20 + 30 + 15));
        }
    }
}
