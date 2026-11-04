using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestResourceTypeTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("network.spec", "network Request.resourceType", "should work for document type")]
        public async Task ShouldWorkForDocumentType()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            var request = response.Request;
            Assert.That(request.ResourceType, Is.EqualTo(ResourceType.Document));
        }

        [Test, PuppeteerTest("network.spec", "network Request.resourceType", "should work for stylesheets")]
        public async Task ShouldWorkForStylesheets()
        {
            var cssRequests = new List<IRequest>();
            var tcs = new TaskCompletionSource<bool>();
            Page.Request += (_, e) =>
            {
                if (e.Request.Url.EndsWith("css"))
                {
                    cssRequests.Add(e.Request);
                    tcs.TrySetResult(true);
                }
            };
            await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            await tcs.Task;
            Assert.That(cssRequests, Has.Exactly(1).Items);
            var request = cssRequests[0];
            Assert.That(request.Url, Does.Contain("one-style.css"));
            Assert.That(request.ResourceType, Is.EqualTo(ResourceType.StyleSheet));
        }
    }
}
