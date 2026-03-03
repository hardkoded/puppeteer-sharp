using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class WaitForFrameTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("page.spec", "Page Page.waitForFrame", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            var waitedFrameTask = Page.WaitForFrameAsync(frame => frame.Url.EndsWith("/title.html"));
            await Task.WhenAll(
                waitedFrameTask,
                FrameUtils.AttachFrameAsync(Page, "frame2", TestConstants.ServerUrl + "/title.html"));

            var waitedFrame = await waitedFrameTask;
            Assert.That(waitedFrame.ParentFrame, Is.EqualTo(Page.MainFrame));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.waitForFrame", "should work with a URL predicate")]
        public async Task ShouldWorkWithAUrlPredicate()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            var waitedFrameTask = Page.WaitForFrameAsync(TestConstants.ServerUrl + "/title.html");
            await Task.WhenAll(
                waitedFrameTask,
                FrameUtils.AttachFrameAsync(Page, "frame2", TestConstants.ServerUrl + "/title.html"));

            var waitedFrame = await waitedFrameTask;
            Assert.That(waitedFrame.ParentFrame, Is.EqualTo(Page.MainFrame));
        }
    }
}
