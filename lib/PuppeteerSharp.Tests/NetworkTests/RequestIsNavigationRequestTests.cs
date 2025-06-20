using CefSharp.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class RequestIsNavigationRequestTests : DevToolsContextBaseTest
    {
        public RequestIsNavigationRequestTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("network.spec.ts", "Request.isNavigationRequest", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            var requests = new Dictionary<string, Request>();
            DevToolsContext.Request += (_, e) => requests[e.Request.Url.Split('/').Last()] = e.Request;
            Server.SetRedirect("/rrredirect", "/frames/one-frame.html");
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/rrredirect");
            Assert.True(requests["rrredirect"].IsNavigationRequest);
            Assert.True(requests["one-frame.html"].IsNavigationRequest);
            Assert.True(requests["frame.html"].IsNavigationRequest);
            Assert.False(requests["script.js"].IsNavigationRequest);
            Assert.False(requests["style.css"].IsNavigationRequest);
        }

        [PuppeteerTest("network.spec.ts", "Request.isNavigationRequest", "should work with request interception")]
        [PuppeteerFact]
        public async Task ShouldWorkWithRequestInterception()
        {
            var requests = new Dictionary<string, Request>();
            DevToolsContext.Request += async (_, e) =>
            {
                requests[e.Request.Url.Split('/').Last()] = e.Request;
                await e.Request.ContinueAsync();
            };

            await DevToolsContext.SetRequestInterceptionAsync(true);
            Server.SetRedirect("/rrredirect", "/frames/one-frame.html");
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/rrredirect");
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
            var requests = new List<Request>();
            DevToolsContext.Request += (_, e) =>
            {
                if (!e.Request.Url.EndsWith("favicon.ico"))
                {
                    requests.Add(e.Request);
                }
            };
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            await ChromiumWebBrowser.WaitForRenderIdleAsync();

            Assert.True(requests[0].IsNavigationRequest);
        }
    }
}
