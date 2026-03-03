using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class TouchStartTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.touchStart", "should work")]
        public async Task ShouldWork()
        {
            await SetupTouchEventReporting();

            await Page.EvaluateExpressionAsync(@"
                document.body.style.padding = '0';
                document.body.style.margin = '0';
                document.body.innerHTML = '<div style=""cursor: pointer; width: 120px; height: 60px; margin: 30px; padding: 15px;""></div>';
            ");

            var divHandle = await Page.QuerySelectorAsync("div");
            await divHandle.TouchStartAsync();

            var events = await Page.EvaluateExpressionAsync<TouchEventReport[]>("window.__touchEvents");

            // margin + middle point offset
            var expectedX = 45 + 60;
            var expectedY = 45 + 30;

            Assert.That(events, Has.Length.EqualTo(1));
            Assert.That(events[0].Changed, Has.Length.EqualTo(1));
            Assert.That(events[0].Changed[0][0], Is.EqualTo(expectedX));
            Assert.That(events[0].Changed[0][1], Is.EqualTo(expectedY));
            Assert.That(events[0].Touches, Has.Length.EqualTo(1));
            Assert.That(events[0].Touches[0][0], Is.EqualTo(expectedX));
            Assert.That(events[0].Touches[0][1], Is.EqualTo(expectedY));
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
