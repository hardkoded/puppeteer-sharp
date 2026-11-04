using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseTimingTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("network.spec", "network Response.timing", "returns timing information")]
        public async Task ReturnsTimingInformation()
        {
            var responses = new List<IResponse>();
            Page.Response += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Response.Request))
                {
                    responses.Add(e.Response);
                }
            };
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(responses, Has.Exactly(1).Items);
            Assert.That(responses[0].Timing.ReceiveHeadersEnd, Is.GreaterThan(0));
        }
    }
}
