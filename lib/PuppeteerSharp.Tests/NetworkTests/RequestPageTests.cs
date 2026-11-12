using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestPageTests : PuppeteerPageBaseTest
    {
        public RequestPageTests() : base()
        {
        }

        [Test, PuppeteerTest("network.spec", "network Request.page", "should work for main frame navigation request")]
        public async Task ShouldWorkForMainFrameNavigationRequest()
        {
            var requestTask = Page.WaitForRequestAsync(TestConstants.EmptyPage);
            await Page.GoToAsync(TestConstants.EmptyPage);
            var request = await requestTask;
            Assert.That(request.Page, Is.SameAs(Page));
        }

        [Test, PuppeteerTest("network.spec", "network Request.page", "should work for subframe navigation request")]
        public async Task ShouldWorkForSubframeNavigationRequest()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var requestTask = Page.WaitForRequestAsync(TestConstants.EmptyPage);
            var frame = await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var request = await requestTask;
            Assert.That(request.Frame, Is.SameAs(frame));
            Assert.That(request.Page, Is.SameAs(Page));
        }

        [Test, PuppeteerTest("network.spec", "network Request.page", "should work for fetch requests")]
        public async Task ShouldWorkForFetchRequests()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var requestTask = Page.WaitForRequestAsync(TestConstants.ServerUrl + "/digits/1.png");
            await Page.EvaluateExpressionAsync("fetch('/digits/1.png')");
            var request = await requestTask;
            Assert.That(request.Page, Is.SameAs(Page));
        }
    }
}
