using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestHeadersTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("network.spec", "network Request.Headers", "should work")]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);

            if (TestConstants.IsChrome)
            {
                Assert.That(response.Request.Headers["User-Agent"], Does.Contain("Chrome"));
            }
            else
            {
                Assert.That(response.Request.Headers["User-Agent"], Does.Contain("Firefox"));
            }
        }
    }
}
