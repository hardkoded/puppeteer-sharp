using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestHeadersTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("network.spec", "network Request.Headers", "should work")]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);

            if (TestConstants.IsChrome)
            {
                StringAssert.Contains("Chrome", response.Request.Headers["User-Agent"]);
            }
            else
            {
                StringAssert.Contains("Firefox", response.Request.Headers["User-Agent"]);
            }
        }
    }
}
