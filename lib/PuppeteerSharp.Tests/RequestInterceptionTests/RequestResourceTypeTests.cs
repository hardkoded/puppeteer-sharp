using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.RequestInterceptionTests
{
    public class RequestResourceTypeTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("requestinterception.spec", "request interception Request.resourceType", "should work for document type")]
        public async Task ShouldWorkForDocumentType()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
            {
                await e.Request.ContinueAsync();
            };
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            var request = response.Request;
            Assert.That(request.ResourceType, Is.EqualTo(ResourceType.Document));
        }

        [Test, PuppeteerTest("requestinterception.spec", "request interception Request.resourceType", "should work for stylesheets")]
        public async Task ShouldWorkForStylesheets()
        {
            await Page.SetRequestInterceptionAsync(true);
            var cssRequests = new List<IRequest>();
            Page.Request += async (_, e) =>
            {
                if (e.Request.Url.EndsWith("css"))
                {
                    cssRequests.Add(e.Request);
                }
                await e.Request.ContinueAsync();
            };
            await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            Assert.That(cssRequests, Has.Count.EqualTo(1));
            var request = cssRequests[0];
            Assert.That(request.Url, Does.Contain("one-style.css"));
            Assert.That(request.ResourceType, Is.EqualTo(ResourceType.StyleSheet));
        }
    }
}
