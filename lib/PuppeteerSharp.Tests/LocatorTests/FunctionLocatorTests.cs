using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Locators;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LocatorTests
{
    public class FunctionLocatorTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("locator.spec", "Locator FunctionLocator", "should work")]
        public async Task ShouldWork()
        {
            var result = await Page.LocatorFunction(
                    @"() => {
                    return new Promise(resolve => {
                        return setTimeout(() => {
                            return resolve(true);
                        }, 100);
                    });
                }"
                )
                .WaitAsync<bool>();

            Assert.That(result, Is.True);
        }

        [Test, PuppeteerTest("locator.spec", "Locator FunctionLocator", "should work with actions")]
        public async Task ShouldWorkWithActions()
        {
            await Page.SetContentAsync("<div onclick=\"window.clicked = true\">test</div>");
            await Page.LocatorFunction("() => document.getElementsByTagName('div')[0]")
                .ClickAsync();

            var clicked = await Page.EvaluateExpressionAsync<bool>("window.clicked");
            Assert.That(clicked, Is.True);
        }

        // Partial port of "should preserve timeout across clone, map, and filter":
        // the port has no public Clone(), so only map and filter are checked.
        [Test]
        public async Task ShouldPreserveTimeoutAcrossMapAndFilter()
        {
            var locator = Page.LocatorFunction("() => false").SetTimeout(100);

            Assert.That(locator.Map("(value) => value").Timeout, Is.EqualTo(100));
            Assert.That(locator.Filter("(value) => Boolean(value)").Timeout, Is.EqualTo(100));
            await Task.CompletedTask;
        }

        [
            Test,
            PuppeteerTest(
                "locator.spec",
                "Locator FunctionLocator",
                "should preserve options across chained setters"
            )
        ]
        public async Task ShouldPreserveOptionsAcrossChainedSetters()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync("<button disabled>test</button>");
            var willClick = false;
            await Page.LocatorFunction("() => document.querySelector('button')")
                .SetWaitForEnabled(false)
                .SetTimeout(2000)
                .On(LocatorEvent.Action, (_, _) => willClick = true)
                .ClickAsync();
            Assert.That(willClick, Is.True);
        }
    }
}
