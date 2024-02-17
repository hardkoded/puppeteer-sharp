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

        [Test, PuppeteerTimeout, PuppeteerTest("network.spec", "network Response.headers", "should work")]
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
