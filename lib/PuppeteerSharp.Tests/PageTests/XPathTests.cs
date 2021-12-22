using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class XPathTests : PuppeteerPageBaseTest
    {
        public XPathTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<section>test</section>");
            var elements = await Page.XPathAsync("/html/body/section");
            Assert.NotNull(elements[0]);
            Assert.Single(elements);
        }

        [Fact]
        public async Task ShouldReturnEmptyArrayForNonExistingElement()
        {
            var elements = await Page.XPathAsync("/html/body/non-existing-element");
            Assert.Empty(elements);
        }

        [Fact]
        public async Task ShouldReturnMultipleElements()
        {
            await Page.SetContentAsync("<div></div><div></div>");
            var elements = await Page.XPathAsync("/html/body/div");
            Assert.Equal(2, elements.Length);
        }
    }
}
