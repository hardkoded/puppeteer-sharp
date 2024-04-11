using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class ElementHandleQuerySelectorTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("queryselector.spec", "ElementHandle.$", "should query existing element")]
        public async Task ShouldQueryExistingElement()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/playground.html");
            await Page.SetContentAsync("<html><body><div class=\"second\"><div class=\"inner\">A</div></div></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var second = await html.QuerySelectorAsync(".second");
            var inner = await second.QuerySelectorAsync(".inner");
            var content = await Page.EvaluateFunctionAsync<string>("e => e.textContent", inner);
            Assert.AreEqual("A", content);
        }

        [Test, Retry(2), PuppeteerTest("queryselector.spec", "ElementHandle.$", "should return null for non-existing element")]
        public async Task ShouldReturnNullForNonExistingElement()
        {
            await Page.SetContentAsync("<html><body><div class=\"second\"><div class=\"inner\">B</div></div></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var second = await html.QuerySelectorAsync(".third");
            Assert.Null(second);
        }

        [Test, Retry(2), PuppeteerTest("queryselector.spec", "ElementHandle.$$ xpath", "should query existing element")]
        public async Task XPathShouldQueryExistingElement()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/playground.html");
            await Page.SetContentAsync("<html><body><div class=\"second\"><div class=\"inner\">A</div></div></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var second = await html.QuerySelectorAllAsync("xpath/./body/div[contains(@class, 'second')]");
            var inner = await second[0].QuerySelectorAllAsync("xpath/./div[contains(@class, 'inner')]");
            var content = await Page.EvaluateFunctionAsync<string>("e => e.textContent", inner[0]);
            Assert.AreEqual("A", content);
        }

        [Test, Retry(2), PuppeteerTest("queryselector.spec", "ElementHandle.$$ xpath", "should return null for non-existing element")]
        public async Task XPathShouldReturnNullForNonExistingElement()
        {
            await Page.SetContentAsync("<html><body><div class=\"second\"><div class=\"inner\">B</div></div></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var second = await html.QuerySelectorAllAsync("xpath/div[contains(@class, 'third')]");
            Assert.IsEmpty(second);
        }
    }
}
