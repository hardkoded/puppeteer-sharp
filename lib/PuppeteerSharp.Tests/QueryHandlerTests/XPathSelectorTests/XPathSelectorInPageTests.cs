using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.QueryHandlerTests.XPathSelectorTests
{
    public class XPathSelectorInPageTests : PuppeteerPageBaseTest
    {
        public XPathSelectorInPageTests() : base()
        {
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests XPath selectors in Page", "should query existing element")]
        public async Task ShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<section>test</section>");
            Assert.That(await Page.QuerySelectorAsync("xpath/html/body/section"), Is.Not.Null);
            Assert.That(await Page.QuerySelectorAllAsync("xpath/html/body/section"), Has.Exactly(1).Items);
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests XPath selectors in Page", "should return empty array for non-existing element")]
        public async Task ShouldReturnEmptyArrayForNonExistingElement()
        {
            Assert.That(await Page.QuerySelectorAsync("xpath/html/body/non-existing-element"), Is.Null);
            Assert.That(await Page.QuerySelectorAllAsync("xpath/html/body/non-existing-element"), Is.Empty);
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests XPath selectors in Page", "should return first element")]
        public async Task ShouldReturnFirstElement()
        {
            await Page.SetContentAsync("<div>a</div><div></div>");
            var element = await Page.QuerySelectorAsync("xpath/html/body/div");
            Assert.That(
                await element.EvaluateFunctionAsync<bool>("e => e.textContent === 'a'"),
                Is.True);
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests XPath selectors in Page", "should return multiple elements")]
        public async Task ShouldReturnMultipleElements()
        {
            await Page.SetContentAsync("<div></div><div></div>");
            var elements = await Page.QuerySelectorAllAsync("xpath/html/body/div");
            Assert.That(elements, Has.Length.EqualTo(2));
        }
    }
}
