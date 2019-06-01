using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class RequestHeadersTests : PuppeteerPageBaseTest
    {
        public RequestHeadersTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Contains("Chrome", response.Request.Headers["User-Agent"]);
        }
    }
}
