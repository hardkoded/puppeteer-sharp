using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class GetElementTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<section>test</section>");
            var element = await Page.GetElementAsync("section");
            Assert.NotNull(element);
        }

        [Fact]
        public async Task ShouldReturnNullForNonExistingElement()
        {
            var element = await Page.GetElementAsync("non-existing-element");
            Assert.Null(element);
        }
    }
}
