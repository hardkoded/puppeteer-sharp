#pragma warning disable CS0618 // WaitForXPathAsync is obsolete but we test the funcionatlity anyway
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class PageXPathTests : PuppeteerPageBaseTest
    {
        public PageXPathTests(): base()
        {
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$x", "should query existing element")]
        [PuppeteerTimeout]
        public async Task ShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<section>test</section>");
            var elements = await Page.XPathAsync("/html/body/section");
            Assert.NotNull(elements[0]);
            Assert.That(elements, Has.Exactly(1).Items);
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$x", "should return empty array for non-existing element")]
        [PuppeteerTimeout]
        public async Task ShouldReturnEmptyArrayForNonExistingElement()
        {
            var elements = await Page.XPathAsync("/html/body/non-existing-element");
            Assert.IsEmpty(elements);
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$x", "should return multiple elements")]
        [PuppeteerTimeout]
        public async Task ShouldReturnMultipleElements()
        {
            await Page.SetContentAsync("<div></div><div></div>");
            var elements = await Page.XPathAsync("/html/body/div");
            Assert.AreEqual(2, elements.Length);
        }
    }
}
#pragma warning restore CS0618