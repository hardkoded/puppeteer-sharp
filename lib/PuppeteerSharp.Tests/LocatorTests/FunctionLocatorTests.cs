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
            var result = await Page
                .LocatorFunction(@"() => {
                    return new Promise(resolve => {
                        return setTimeout(() => {
                            return resolve(true);
                        }, 100);
                    });
                }")
                .WaitAsync<bool>();

            Assert.That(result, Is.True);
        }

        [Test, PuppeteerTest("locator.spec", "Locator FunctionLocator", "should work with actions")]
        public async Task ShouldWorkWithActions()
        {
            await Page.SetContentAsync("<div onclick=\"window.clicked = true\">test</div>");
            await Page
                .LocatorFunction("() => document.getElementsByTagName('div')[0]")
                .ClickAsync();

            var clicked = await Page.EvaluateExpressionAsync<bool>("window.clicked");
            Assert.That(clicked, Is.True);
        }
    }
}
