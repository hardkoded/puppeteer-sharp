using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class RequestHeadersTests : DevToolsContextBaseTest
    {
        public RequestHeadersTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("network.spec.ts", "Request.Headers", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage);

            Assert.Contains("Chrome", response.Request.Headers["User-Agent"]);
        }
    }
}
