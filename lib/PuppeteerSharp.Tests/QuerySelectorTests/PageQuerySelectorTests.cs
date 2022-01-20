using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Xunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageQuerySelectorTests : PuppeteerPageBaseTest
    {
        public PageQuerySelectorTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$", "should query existing element")]
        [PuppeteerFact]
        public async Task ShouldQueryExistingElement()
        {
            await DevToolsContext.SetContentAsync("<section>test</section>");
            var element = await DevToolsContext.QuerySelectorAsync("section");
            Assert.NotNull(element);
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$", "should query existing element")]
        [PuppeteerFact]
        public async Task ShouldReturnNullForNonExistingElement()
        {
            var element = await DevToolsContext.QuerySelectorAsync("non-existing-element");
            Assert.Null(element);
        }
    }
}
