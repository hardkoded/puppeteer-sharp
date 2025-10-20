using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestIsNavigationRequestTests : PuppeteerPageBaseTest
    {
        public RequestIsNavigationRequestTests() : base()
        {
        }

        [Test, PuppeteerTest("network.spec", "network Request.isNavigationRequest", "should work")]
        public async Task ShouldWork()
        {
            var requests = new Dictionary<string, IRequest>();
            Page.Request += (_, e) => requests[e.Request.Url.Split('/').Last()] = e.Request;
            Server.SetRedirect("/rrredirect", "/frames/one-frame.html");
            await Page.GoToAsync(TestConstants.ServerUrl + "/rrredirect");
            Assert.That(requests["rrredirect"].IsNavigationRequest, Is.True);
            Assert.That(requests["one-frame.html"].IsNavigationRequest, Is.True);
            Assert.That(requests["frame.html"].IsNavigationRequest, Is.True);
            Assert.That(requests["script.js"].IsNavigationRequest, Is.False);
            Assert.That(requests["style.css"].IsNavigationRequest, Is.False);
        }

        [Test, PuppeteerTest("network.spec", "network Request.isNavigationRequest", "should work with request interception")]
        public async Task ShouldWorkWithRequestInterception()
        {
            var requests = new Dictionary<string, IRequest>();
            Page.Request += async (_, e) =>
            {
                requests[e.Request.Url.Split('/').Last()] = e.Request;
                await e.Request.ContinueAsync();
            };

            await Page.SetRequestInterceptionAsync(true);
            Server.SetRedirect("/rrredirect", "/frames/one-frame.html");
            await Page.GoToAsync(TestConstants.ServerUrl + "/rrredirect");
            Assert.That(requests["rrredirect"].IsNavigationRequest, Is.True);
            Assert.That(requests["one-frame.html"].IsNavigationRequest, Is.True);
            Assert.That(requests["frame.html"].IsNavigationRequest, Is.True);
            Assert.That(requests["script.js"].IsNavigationRequest, Is.False);
            Assert.That(requests["style.css"].IsNavigationRequest, Is.False);
        }

        [Test, PuppeteerTest("network.spec", "network Request.isNavigationRequest", "should work when navigating to image")]
        public async Task ShouldWorkWhenNavigatingToImage()
        {
            var requests = new List<IRequest>();
            Page.Request += (_, e) => requests.Add(e.Request);
            await Page.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            Assert.That(requests[0].IsNavigationRequest, Is.True);
        }
    }
}
