using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Xunit;
using PuppeteerSharp.Tests.Attributes;
using CefSharp.Puppeteer;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class RequestFrameTests : PuppeteerPageBaseTest
    {
        public RequestFrameTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("network.spec.ts", "Request.Frame", "should work for main frame navigation request")]
        [PuppeteerFact]
        public async Task ShouldWorkForMainFrameNavigationRequests()
        {
            var requests = new List<Request>();
            Page.Request += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Single(requests);
            Assert.Equal(Page.MainFrame, requests[0].Frame);
        }

        [PuppeteerTest("network.spec.ts", "Request.Frame", "should work for subframe navigation request")]
        [PuppeteerFact]
        public async Task ShouldWorkForSubframeNavigationRequest()
        {
            var requests = new List<Request>();
            Page.Request += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };

            await Page.GoToAsync(TestConstants.EmptyPage);

            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            Assert.Equal(2, requests.Count);
            Assert.Equal(Page.FirstChildFrame(), requests[1].Frame);
        }

        [PuppeteerTest("network.spec.ts", "Request.Frame", "should work for fetch requests")]
        [PuppeteerFact]
        public async Task ShouldWorkForFetchRequests()
        {
            var requests = new List<Request>();
            Page.Request += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };

            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateExpressionAsync("fetch('/empty.html')");
            Assert.Equal(2, requests.Count);
            Assert.Equal(Page.MainFrame, requests[0].Frame);
        }
    }
}
