#pragma warning disable CS0618 // XPathAsync is obsolete but we test the funcionatlity anyway
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class ElementHandleXPathTests : PuppeteerPageBaseTest
    {
        public ElementHandleXPathTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("queryselector.spec", "ElementHandle.$x", "should query existing element")]
        public async Task ShouldQueryExistingElement()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/playground.html");
            await Page.SetContentAsync("<html><body><div class=\"second\"><div class=\"inner\">A</div></div></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var second = await html.XPathAsync("./body/div[contains(@class, 'second')]");
            var inner = await second[0].XPathAsync("./div[contains(@class, 'inner')]");
            var content = await Page.EvaluateFunctionAsync<string>("e => e.textContent", inner[0]);
            Assert.AreEqual("A", content);
        }

        [Test, Retry(2), PuppeteerTest("queryselector.spec", "ElementHandle.$x", "should return null for non-existing element")]
        public async Task ShouldReturnNullForNonExistingElement()
        {
            await Page.SetContentAsync("<html><body><div class=\"second\"><div class=\"inner\">B</div></div></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var second = await html.XPathAsync("/div[contains(@class, 'third')]");
            Assert.IsEmpty(second);
        }
    }
}
#pragma warning restore CS0618
