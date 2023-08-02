using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class RequestIsNavigationRequestTests : PuppeteerPageBaseTest
    {
        public RequestIsNavigationRequestTests(): base()
        {
        }

        [PuppeteerTest("network.spec.ts", "Request.isNavigationRequest", "should work")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            var requests = new Dictionary<string, IRequest>();
            Page.Request += (_, e) => requests[e.Request.Url.Split('/').Last()] = e.Request;
            Server.SetRedirect("/rrredirect", "/frames/one-frame.html");
            await Page.GoToAsync(TestConstants.ServerUrl + "/rrredirect");
            Assert.True(requests["rrredirect"].IsNavigationRequest);
            Assert.True(requests["one-frame.html"].IsNavigationRequest);
            Assert.True(requests["frame.html"].IsNavigationRequest);
            Assert.False(requests["script.js"].IsNavigationRequest);
            Assert.False(requests["style.css"].IsNavigationRequest);
        }

        [PuppeteerTest("network.spec.ts", "Request.isNavigationRequest", "should work with request interception")]
        [SkipBrowserFact(skipFirefox: true)]
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
            Assert.True(requests["rrredirect"].IsNavigationRequest);
            Assert.True(requests["one-frame.html"].IsNavigationRequest);
            Assert.True(requests["frame.html"].IsNavigationRequest);
            Assert.False(requests["script.js"].IsNavigationRequest);
            Assert.False(requests["style.css"].IsNavigationRequest);
        }

        [PuppeteerTest("network.spec.ts", "Request.isNavigationRequest", "should work when navigating to image")]
        [PuppeteerFact]
        public async Task ShouldWorkWhenNavigatingToImage()
        {
            var requests = new List<IRequest>();
            Page.Request += (_, e) => requests.Add(e.Request);
            await Page.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            Assert.True(requests[0].IsNavigationRequest);
        }
    }
}
