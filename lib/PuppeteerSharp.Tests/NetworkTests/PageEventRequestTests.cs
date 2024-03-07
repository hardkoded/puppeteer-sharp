using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class PageEventRequestTests : PuppeteerPageBaseTest
    {
        public PageEventRequestTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Page.Events.Request", "should fire for navigation requests")]
        public async Task ShouldFireForNavigationRequests()
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
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Page.Events.Request", "should fire for iframes")]
        public async Task ShouldFireForIframes()
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
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Page.Events.Request", "should fire for fetches")]
        public async Task ShouldFireForFetches()
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
        }
    }
}
