using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class PageQuerySelectorTests : PuppeteerPageBaseTest
    {
        public PageQuerySelectorTests() : base()
        {
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$", "should query existing element")]
        [PuppeteerTimeout]
        public async Task ShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<section>test</section>");
            var element = await Page.QuerySelectorAsync("section");
            Assert.NotNull(element);
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$", "should return null for non-existing element")]
        [PuppeteerTimeout]
        public async Task ShouldReturnNullForNonExistingElement()
        {
            var element = await Page.QuerySelectorAsync("non-existing-element");
            Assert.Null(element);
        }
    }
}
