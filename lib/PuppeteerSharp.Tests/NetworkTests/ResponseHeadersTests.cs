using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseHeadersTests : PuppeteerPageBaseTest
    {
        public ResponseHeadersTests(): base()
        {
        }

        [PuppeteerTest("network.spec.ts", "Response.headers", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            Server.SetRoute("/empty.html", (context) =>
            {
                context.Response.Headers["foo"] = "bar";
                return Task.CompletedTask;
            });

            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Contains("bar", response.Headers["foo"]);
        }
    }
}
