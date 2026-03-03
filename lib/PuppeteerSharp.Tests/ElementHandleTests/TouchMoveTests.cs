using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class TouchMoveTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.touchMove", "should work")]
        public async Task ShouldWork()
        {
            await SetupTouchEventReporting();

            await Page.EvaluateExpressionAsync(@"
                document.body.style.padding = '0';
                document.body.style.margin = '0';
                document.body.innerHTML = '<div style=""cursor: pointer; width: 120px; height: 60px; margin: 30px; padding: 15px;""></div>';
            ");

            var divHandle = await Page.QuerySelectorAsync("div");
            await Page.Touchscreen.TouchStartAsync(200, 200);
            await divHandle.TouchMoveAsync();

            var events = await Page.EvaluateExpressionAsync<TouchEventReport[]>("window.__touchEvents");

            // margin + middle point offset
            var expectedDivX = 45 + 60;
            var expectedDivY = 45 + 30;

            Assert.That(events, Has.Length.EqualTo(2));

            // First event: touchstart at (200, 200)
            Assert.That(events[0].Changed[0][0], Is.EqualTo(200));
            Assert.That(events[0].Changed[0][1], Is.EqualTo(200));
            Assert.That(events[0].Touches[0][0], Is.EqualTo(200));
            Assert.That(events[0].Touches[0][1], Is.EqualTo(200));

            // Second event: touchmove to the div center
            Assert.That(events[1].Changed[0][0], Is.EqualTo(expectedDivX));
            Assert.That(events[1].Changed[0][1], Is.EqualTo(expectedDivY));
            Assert.That(events[1].Touches[0][0], Is.EqualTo(expectedDivX));
            Assert.That(events[1].Touches[0][1], Is.EqualTo(expectedDivY));
        }

        private async Task SetupTouchEventReporting()
        {
            await Page.EvaluateExpressionAsync(@"
                window.__touchEvents = [];
                document.body.addEventListener('touchstart', reportTouchEvent);
                document.body.addEventListener('touchmove', reportTouchEvent);
                document.body.addEventListener('touchend', reportTouchEvent);
                function reportTouchEvent(e) {
                    window.__touchEvents.push({
                        changed: [...e.changedTouches].map(t => [t.pageX, t.pageY]),
                        touches: [...e.touches].map(t => [t.pageX, t.pageY]),
                    });
                }
            ");
        }

        public class TouchEventReport
        {
            public decimal[][] Changed { get; set; }

            public decimal[][] Touches { get; set; }
        }
    }
}
