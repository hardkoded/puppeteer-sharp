#pragma warning disable CS0618 // WaitForXPathAsync is obsolete but we test the funcionatlity anyway
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class PageXPathTests : PuppeteerPageBaseTest
    {
        public PageXPathTests() : base()
        {
        }

        [Test, PuppeteerTimeout, PuppeteerTest("queryselector.spec", "Page.$x", "should query existing element")]
        public async Task ShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<section>test</section>");
            var elements = await Page.XPathAsync("/html/body/section");
            Assert.NotNull(elements[0]);
            Assert.That(elements, Has.Exactly(1).Items);
        }

        [Test, PuppeteerTimeout, PuppeteerTest("queryselector.spec", "Page.$x", "should return empty array for non-existing element")]
        public async Task ShouldReturnEmptyArrayForNonExistingElement()
        {
            var elements = await Page.XPathAsync("/html/body/non-existing-element");
            Assert.IsEmpty(elements);
        }

        [Test, PuppeteerTimeout, PuppeteerTest("queryselector.spec", "Page.$x", "should return multiple elements")]
        public async Task ShouldReturnMultipleElements()
        {
            await Page.SetContentAsync("<div></div><div></div>");
            var elements = await Page.XPathAsync("/html/body/div");
            Assert.AreEqual(2, elements.Length);
        }
    }
}
#pragma warning restore CS0618
