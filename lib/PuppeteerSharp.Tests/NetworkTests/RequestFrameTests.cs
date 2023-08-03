using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestFrameTests : PuppeteerPageBaseTest
    {
        public RequestFrameTests(): base()
        {
        }

        [PuppeteerTest("network.spec.ts", "Request.Frame", "should work for main frame navigation request")]
        [PuppeteerTimeout]
        public async Task ShouldWorkForMainFrameNavigationRequests()
        {
            var requests = new List<IRequest>();
            Page.Request += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(requests, Has.Exactly(1).Items);
            Assert.AreEqual(Page.MainFrame, requests[0].Frame);
        }

        [PuppeteerTest("network.spec.ts", "Request.Frame", "should work for subframe navigation request")]
        [PuppeteerTimeout]
        public async Task ShouldWorkForSubframeNavigationRequest()
        {
            var requests = new List<IRequest>();
            Page.Request += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };

            await Page.GoToAsync(TestConstants.EmptyPage);

            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual(Page.FirstChildFrame(), requests[1].Frame);
        }

        [PuppeteerTest("network.spec.ts", "Request.Frame", "should work for fetch requests")]
        [PuppeteerTimeout]
        public async Task ShouldWorkForFetchRequests()
        {
            var requests = new List<IRequest>();
            Page.Request += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };

            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateExpressionAsync("fetch('/empty.html')");
            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual(Page.MainFrame, requests[0].Frame);
        }
    }
}
