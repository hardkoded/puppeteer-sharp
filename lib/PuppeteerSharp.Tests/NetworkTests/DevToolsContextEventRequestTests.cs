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
    public class DevToolsContextEventRequestTests : DevToolsContextBaseTest
    {
        public DevToolsContextEventRequestTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("network.spec.ts", "Page.Events.Request", "should fire for navigation requests")]
        [PuppeteerFact]
        public async Task ShouldFireForNavigationRequests()
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
        }

        [PuppeteerTest("network.spec.ts", "Page.Events.Request", "should fire for iframes")]
        [PuppeteerFact]
        public async Task ShouldFireForIframes()
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
        }

        [PuppeteerTest("network.spec.ts", "Page.Events.Request", "should fire for fetches")]
        [PuppeteerFact]
        public async Task ShouldFireForFetches()
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
        }
    }
}
