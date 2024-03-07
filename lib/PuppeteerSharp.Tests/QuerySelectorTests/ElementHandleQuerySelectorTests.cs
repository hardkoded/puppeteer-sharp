using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class ElementHandleQuerySelectorTests : PuppeteerPageBaseTest
    {
        public ElementHandleQuerySelectorTests() : base()
        {
        }

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
    }
}
