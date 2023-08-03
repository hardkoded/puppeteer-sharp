using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseHeadersTests : PuppeteerPageBaseTest
    {
        public ResponseHeadersTests(): base()
        {
        }

        [PuppeteerTest("network.spec.ts", "Response.headers", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
        {
            Server.SetRoute("/empty.html", (context) =>
            {
                context.Response.Headers["foo"] = "bar";
                return Task.CompletedTask;
            });

            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            StringAssert.Contains("bar", response.Headers["foo"]);
        }
    }
}
