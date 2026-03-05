using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.RequestInterceptionExperimentalTests;

public class RequestResourceTypeTests : PuppeteerPageBaseTest
{
    [Test, PuppeteerTest("requestinterception-experimental.spec", "Request.resourceType", "should work for document type")]
    public async Task ShouldWorkForDocumentType()
    {
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request => request.ContinueAsync(new Payload(), 0));
        var response = await Page.GoToAsync(TestConstants.EmptyPage);
        var request = response.Request;
        Assert.That(request.ResourceType, Is.EqualTo(ResourceType.Document));
    }

    [Test, PuppeteerTest("requestinterception-experimental.spec", "Request.resourceType", "should work for stylesheets")]
    public async Task ShouldWorkForStylesheets()
    {
        await Page.SetRequestInterceptionAsync(true);
        var cssRequests = new List<IRequest>();
        Page.AddRequestInterceptor(request =>
        {
            if (request.Url.EndsWith("css"))
            {
                cssRequests.Add(request);
            }

            return request.ContinueAsync(new Payload(), 0);
        });
        await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
        Assert.That(cssRequests, Has.Count.EqualTo(1));
        var request = cssRequests[0];
        Assert.That(request.Url, Does.Contain("one-style.css"));
        Assert.That(request.ResourceType, Is.EqualTo(ResourceType.StyleSheet));
    }
}
