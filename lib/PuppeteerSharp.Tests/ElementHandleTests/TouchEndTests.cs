using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class TouchEndTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.touchEnd", "should work")]
        public async Task ShouldWork()
        {
            await SetupTouchEventReporting();

            await Page.EvaluateExpressionAsync(@"
                document.body.style.padding = '0';
                document.body.style.margin = '0';
                document.body.innerHTML = '<div style=""cursor: pointer; width: 120px; height: 60px; margin: 30px; padding: 15px;""></div>';
            ");

            var divHandle = await Page.QuerySelectorAsync("div");
            await Page.Touchscreen.TouchStartAsync(100, 100);
            await divHandle.TouchEndAsync();

            var events = await Page.EvaluateExpressionAsync<TouchEventReport[]>("window.__touchEvents");

            Assert.That(events, Has.Length.EqualTo(2));

            // First event: touchstart at (100, 100)
            Assert.That(events[0].Changed[0][0], Is.EqualTo(100));
            Assert.That(events[0].Changed[0][1], Is.EqualTo(100));
            Assert.That(events[0].Touches[0][0], Is.EqualTo(100));
            Assert.That(events[0].Touches[0][1], Is.EqualTo(100));

            // Second event: touchend
            Assert.That(events[1].Changed[0][0], Is.EqualTo(100));
            Assert.That(events[1].Changed[0][1], Is.EqualTo(100));
            Assert.That(events[1].Touches, Has.Length.EqualTo(0));
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
