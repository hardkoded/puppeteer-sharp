using System.Collections.Generic;
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

        [Test, PuppeteerTest("network.spec", "network Request.Frame", "should work for main frame navigation request")]
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
            Assert.That(requests[0].Frame, Is.EqualTo(Page.MainFrame));
        }

        [Test, PuppeteerTest("network.spec", "network Request.Frame", "should work for subframe navigation request")]
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
            Assert.That(requests, Has.Count.EqualTo(2));
            Assert.That(requests[1].Frame, Is.EqualTo(Page.FirstChildFrame()));
        }

        [Test, PuppeteerTest("network.spec", "network Request.Frame", "should work for fetch requests")]
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
            Assert.That(requests, Has.Count.EqualTo(2));
            Assert.That(requests[0].Frame, Is.EqualTo(Page.MainFrame));
        }
    }
}
