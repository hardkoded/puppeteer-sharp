using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestHeadersTests : PuppeteerPageBaseTest
    {
        public RequestHeadersTests() : base()
        {
        }

        [Test, PuppeteerTest("network.spec", "network Request.Headers", "should work")]
        [PuppeteerTimeout]
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
