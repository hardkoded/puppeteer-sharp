using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class RequestHeadersTests : PuppeteerPageBaseTest
    {
        public RequestHeadersTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);

            if (TestConstants.IsChrome)
            {
                Assert.Contains("Chrome", response.Request.Headers["User-Agent"]);
            }
            else
            {
                Assert.Contains("Firefox", response.Request.Headers["User-Agent"]);
            }
        }
    }
}
