#pragma warning disable CS0618 // WaitForXPathAsync is obsolete but we test the funcionatlity anyway
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class PageXPathTests : PuppeteerPageBaseTest
    {
        public PageXPathTests(): base()
        {
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$x", "should query existing element")]
        [PuppeteerFact]
        public async Task ShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<section>test</section>");
            var elements = await Page.XPathAsync("/html/body/section");
            Assert.NotNull(elements[0]);
            Assert.Single(elements);
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$x", "should return empty array for non-existing element")]
        [PuppeteerFact]
        public async Task ShouldReturnEmptyArrayForNonExistingElement()
        {
            var elements = await Page.XPathAsync("/html/body/non-existing-element");
            Assert.Empty(elements);
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$x", "should return multiple elements")]
        [PuppeteerFact]
        public async Task ShouldReturnMultipleElements()
        {
            await Page.SetContentAsync("<div></div><div></div>");
            var elements = await Page.XPathAsync("/html/body/div");
            Assert.Equal(2, elements.Length);
        }
    }
}
#pragma warning restore CS0618