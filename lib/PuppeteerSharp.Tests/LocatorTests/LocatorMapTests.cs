using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LocatorTests
{
    public class LocatorMapTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("locator.spec", "Locator.prototype.map", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<div>test</div>");
            var result = await Page
                .Locator("div")
                .Map("(element) => element.getAttribute('clickable')")
                .WaitAsync<string>();

            Assert.That(result, Is.Null);

            await Page.EvaluateExpressionAsync(
                "document.querySelector('div')?.setAttribute('clickable', 'true')");

            var result2 = await Page
                .Locator("div")
                .Map("(element) => element.getAttribute('clickable')")
                .WaitAsync<string>();

            Assert.That(result2, Is.EqualTo("true"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator.prototype.map", "should work with throws")]
        public async Task ShouldWorkWithThrows()
        {
            await Page.SetContentAsync("<div>test</div>");
            var resultTask = Page
                .Locator("div")
                .SetTimeout(5000)
                .Map(@"(element) => {
                    const clickable = element.getAttribute('clickable');
                    if (!clickable) {
                        throw new Error('Missing `clickable` as an attribute');
                    }
                    return clickable;
                }")
                .WaitAsync<string>();

            await Task.Delay(2000);
            await Page.EvaluateExpressionAsync(
                "document.querySelector('div')?.setAttribute('clickable', 'true')");

            var result = await resultTask;
            Assert.That(result, Is.EqualTo("true"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator.prototype.map", "should work with expect")]
        public async Task ShouldWorkWithExpect()
        {
            await Page.SetContentAsync("<div>test</div>");
            var resultTask = Page
                .Locator("div")
                .SetTimeout(5000)
                .Filter("(element) => element.getAttribute('clickable') !== null")
                .Map("(element) => element.getAttribute('clickable')")
                .WaitAsync<string>();

            await Task.Delay(2000);
            await Page.EvaluateExpressionAsync(
                "document.querySelector('div')?.setAttribute('clickable', 'true')");

            var result = await resultTask;
            Assert.That(result, Is.EqualTo("true"));
        }
    }
}
