using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Xunit;
using PuppeteerSharp.Tests.Attributes;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class RequestFrameTests : DevToolsContextBaseTest
    {
        public RequestFrameTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("network.spec.ts", "Request.Frame", "should work for main frame navigation request")]
        [PuppeteerFact]
        public async Task ShouldWorkForMainFrameNavigationRequests()
        {
            var requests = new List<Request>();
            DevToolsContext.Request += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };

            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Single(requests);
            Assert.Equal(DevToolsContext.MainFrame, requests[0].Frame);
        }

        [PuppeteerTest("network.spec.ts", "Request.Frame", "should work for subframe navigation request")]
        [PuppeteerFact]
        public async Task ShouldWorkForSubframeNavigationRequest()
        {
            var requests = new List<Request>();
            DevToolsContext.Request += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };

            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);

            await FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", TestConstants.EmptyPage);
            Assert.Equal(2, requests.Count);
            Assert.Equal(DevToolsContext.FirstChildFrame(), requests[1].Frame);
        }

        [PuppeteerTest("network.spec.ts", "Request.Frame", "should work for fetch requests")]
        [PuppeteerFact]
        public async Task ShouldWorkForFetchRequests()
        {
            var requests = new List<Request>();
            DevToolsContext.Request += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };

            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.EvaluateExpressionAsync("fetch('/empty.html')");
            Assert.Equal(2, requests.Count);
            Assert.Equal(DevToolsContext.MainFrame, requests[0].Frame);
        }
    }
}
