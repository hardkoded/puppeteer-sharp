using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseHeadersTests : PuppeteerPageBaseTest
    {
        public ResponseHeadersTests() : base()
        {
        }

        [Test, PuppeteerTest("network.spec", "Response.headers", "should work")]
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
