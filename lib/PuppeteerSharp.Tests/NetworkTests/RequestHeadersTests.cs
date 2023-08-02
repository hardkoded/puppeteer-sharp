using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class RequestHeadersTests : PuppeteerPageBaseTest
    {
        public RequestHeadersTests(): base()
        {
        }

        [PuppeteerTest("network.spec.ts", "Request.Headers", "should work")]
        [PuppeteerFact]
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
