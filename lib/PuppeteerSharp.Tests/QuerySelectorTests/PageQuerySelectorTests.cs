using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class PageQuerySelectorTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("queryselector.spec", "Page.$", "should query existing element")]
        public async Task ShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<section>test</section>");
            var element = await Page.QuerySelectorAsync("section");
            Assert.That(element, Is.Not.Null);
        }

        [Test, PuppeteerTest("queryselector.spec", "Page.$", "should return null for non-existing element")]
        public async Task ShouldReturnNullForNonExistingElement()
        {
            var element = await Page.QuerySelectorAsync("non-existing-element");
            Assert.That(element, Is.Null);
        }

        [Test, PuppeteerTest("queryselector.spec", "querySelector Page.$$ xpath", "should query existing element")]
        public async Task XPathShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<section>test</section>");
            var elements = await Page.QuerySelectorAllAsync("xpath/html/body/section");
            Assert.That(elements[0], Is.Not.Null);
            Assert.That(elements, Has.Exactly(1).Items);
        }

        [Test, PuppeteerTest("queryselector.spec", "querySelector Page.$$ xpath", "should return empty array for non-existing element")]
        public async Task ShouldReturnEmptyArrayForNonExistingElement()
        {
            var elements = await Page.QuerySelectorAllAsync("xpath/html/body/non-existing-element");
            Assert.That(elements, Is.Empty);
        }

        [Test, PuppeteerTest("queryselector.spec", "querySelector Page.$$ xpath", "should return multiple elements")]
        public async Task XpathShouldReturnMultipleElements()
        {
            await Page.SetContentAsync("<div></div><div></div>");
            var elements = await Page.QuerySelectorAllAsync("xpath/html/body/div");
            Assert.That(elements, Has.Length.EqualTo(2));
        }
    }
}
