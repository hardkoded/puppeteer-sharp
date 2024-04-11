using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class PageQuerySelectorTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("queryselector.spec", "Page.$", "should query existing element")]
        public async Task ShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<section>test</section>");
            var element = await Page.QuerySelectorAsync("section");
            Assert.NotNull(element);
        }

        [Test, Retry(2), PuppeteerTest("queryselector.spec", "Page.$", "should return null for non-existing element")]
        public async Task ShouldReturnNullForNonExistingElement()
        {
            var element = await Page.QuerySelectorAsync("non-existing-element");
            Assert.Null(element);
        }

        [Test, Retry(2), PuppeteerTest("queryselector.spec", "querySelector Page.$$ xpath", "should query existing element")]
        public async Task XPathShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<section>test</section>");
            var elements = await Page.QuerySelectorAllAsync("xpath/html/body/section");
            Assert.NotNull(elements[0]);
            Assert.That(elements, Has.Exactly(1).Items);
        }

        [Test, Retry(2), PuppeteerTest("queryselector.spec", "querySelector Page.$$ xpath", "should return empty array for non-existing element")]
        public async Task ShouldReturnEmptyArrayForNonExistingElement()
        {
            var elements = await Page.QuerySelectorAllAsync("xpath/html/body/non-existing-element");
            Assert.IsEmpty(elements);
        }

        [Test, Retry(2), PuppeteerTest("queryselector.spec", "querySelector Page.$$ xpath", "should return multiple elements")]
        public async Task XpathShouldReturnMultipleElements()
        {
            await Page.SetContentAsync("<div></div><div></div>");
            var elements = await Page.QuerySelectorAllAsync("xpath/html/body/div");
            Assert.AreEqual(2, elements.Length);
        }
    }
}
