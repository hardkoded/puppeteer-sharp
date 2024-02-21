using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestFrameTests : PuppeteerPageBaseTest
    {
        public RequestFrameTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Request.Frame", "should work for main frame navigation request")]
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

        [Test, Retry(2), PuppeteerTest("network.spec", "network Request.Frame", "should work for subframe navigation request")]
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

        [Test, Retry(2), PuppeteerTest("network.spec", "network Request.Frame", "should work for fetch requests")]
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
