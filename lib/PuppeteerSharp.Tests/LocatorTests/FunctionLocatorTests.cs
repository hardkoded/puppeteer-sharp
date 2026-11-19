using System.Threading.Tasks;
using NUnit.Framework;
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

        [
            Test,
            PuppeteerTest(
                "locator.spec",
                "Locator FunctionLocator",
                "should preserve timeout across clone, map, and filter"
            )
        ]
        public void ShouldPreserveTimeoutAcrossCloneMapAndFilter()
        {
            var locator = Page.LocatorFunction("() => false").SetTimeout(100);

            Assert.That(locator.Clone().Timeout, Is.EqualTo(100));
            Assert.That(locator.Map("value => value").Timeout, Is.EqualTo(100));
            Assert.That(locator.Filter("value => Boolean(value)").Timeout, Is.EqualTo(100));
        }
    }
}
